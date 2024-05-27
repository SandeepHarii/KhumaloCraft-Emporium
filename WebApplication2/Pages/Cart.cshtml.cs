using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using KhumaloCraft2.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;

namespace KhumaloCraft2.Pages
{
    public class CartModel : PageModel
    {
        public string SuccessMessage { get; set; } = "";
        private readonly string _connectionString;
        private readonly UserManager<ApplicationUser> _userManager;
        public string UserName { get; private set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        public CartModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _connectionString = "Server=tcp:st10050442-part2.database.windows.net,1433;Initial Catalog=KhumaloCraft_DB;Persist Security Info=False;User ID=Sandeep;Password=H@numan108;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);
            UserName = currentUser.UserName; // Sets the username here
            try
            {
                await FetchCartItemsFromDatabaseAsync();
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching cart items: " + ex.Message);
                Console.WriteLine("Redirecting to error page...");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostConfirmOrderAsync()
        {
            try
            {
                await FetchCartItemsFromDatabaseAsync(); // Fetch cart items before inserting into orders
                int newOrderID = await GetMaxOrderID() + 1; // Get the maximum order ID and increment by 1
                await InsertCartItemsIntoOrderAsync(newOrderID); // Insert cart items into orders with the new order ID
                return RedirectToPage("/add-view-order", new { cartItems = CartItems });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error processing order: " + ex.Message);
                Console.WriteLine("Redirecting to error page...");
                return RedirectToPage("/error");
            }
        }

        private async Task<int> GetMaxOrderID()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sqlGetOrderID = "SELECT ISNULL(MAX(OrderID), 0) FROM OrderDetails";

                    Console.WriteLine();
                    Console.WriteLine("Executing SELECT for MAX OrderID: ");
                    Console.WriteLine(sqlGetOrderID);
                    Console.WriteLine();

                    using (SqlCommand command = new SqlCommand(sqlGetOrderID, connection))
                    {
                        int maxOrderID = (int)await command.ExecuteScalarAsync();
                        Console.WriteLine("Max OrderID = " + maxOrderID);
                        return maxOrderID;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred while fetching the maximum order ID: " + ex.Message);
                throw; // Rethrow the exception to propagate it to the caller
            }
        }

        private async Task FetchCartItemsFromDatabaseAsync()
        {
            try
            {
                ApplicationUser currentUser = await _userManager.GetUserAsync(User);

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "SELECT C.CartID, P.ProductID, PH.Image, P.ProductName, C.ProductQuantity, P.ProductPrice, C.ProductSubTotal, C.UserName, C.DateAdded FROM Cart C LEFT JOIN Products P ON C.ProductID = P.ProductID LEFT JOIN Photos PH ON P.ProductID = PH.ProductID WHERE C.UserName = @UserName";

                    Console.WriteLine();
                    Console.WriteLine("Executing SELECT from CART for USER: ");
                    Console.WriteLine(sql);

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", currentUser.UserName);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                CartItem cartItem = new CartItem
                                {
                                    CartID = reader.GetInt32(0),
                                    ProductID = reader.GetInt32(1),
                                    Image = reader.IsDBNull(2) ? null : (byte[])reader["Image"],
                                    ProductName = reader.GetString(3),
                                    ProductQuantity = reader.GetInt32(4),
                                    ProductPrice = reader.GetDecimal(5),
                                    ProductSubTotal = reader.GetDecimal(6),
                                    UserName = reader.GetString(7),
                                    DateAdded = reader.GetDateTime(8)
                                };

                                CartItems.Add(cartItem);
                            }
                        }
                    }
                }

                Console.WriteLine($"Fetched {CartItems.Count} cart items from the database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred while fetching cart items from database: " + ex.Message);
                throw; // Rethrow the exception to propagate it to the caller
            }
        }

        private async Task InsertCartItemsIntoOrderAsync(int newOrderID)
        {
            try
            {
                ApplicationUser currentUser = await _userManager.GetUserAsync(User);

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Set IDENTITY_INSERT ON
                    string setIdentityInsertOn = "SET IDENTITY_INSERT OrderDetails ON";
                    using (SqlCommand cmd = new SqlCommand(setIdentityInsertOn, connection))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }

                    // Calculate the total order amount
                    decimal orderTotal = 0;
                    foreach (var cartItem in CartItems)
                    {
                        orderTotal += cartItem.ProductSubTotal; // Accumulate subtotal for each item
                    }

                    // Now that we have the order total, insert order details into the database
                    foreach (var cartItem in CartItems)
                    {
                        string sql = "INSERT INTO OrderDetails (OrderID, ProductID, ProductName, ProductQuantity, ProductPrice, ProductSubTotal, orderTotal, UserName, OrderStatus, OrderDate) " +
                                     "VALUES (@OrderID, @ProductId, @ProductName, @ProductQuantity, @ProductPrice, @ProductSubTotal, @orderTotal, @UserName, @OrderStatus, @OrderDate)";

                        Console.WriteLine();
                        Console.WriteLine("***************************************************************");
                        Console.WriteLine();
                        Console.WriteLine("Executing INSERT into ORDER DETAILS: ");
                        Console.WriteLine(sql);
                        Console.WriteLine($"Values: OrderID={newOrderID}, ProductID={cartItem.ProductID}, ProductName={cartItem.ProductName}, ProductQuantity={cartItem.ProductQuantity}, " +
                                          $"ProductPrice={cartItem.ProductPrice}, ProductSubTotal={cartItem.ProductSubTotal}, orderTotal={orderTotal}, UserName={cartItem.UserName}, OrderStatus='Order Placed', " +
                                          $"OrderDate={DateTime.Now}");
                        Console.WriteLine();
                        Console.WriteLine("***************************************************************");
                        Console.WriteLine();

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@OrderID", newOrderID);
                            command.Parameters.AddWithValue("@ProductId", cartItem.ProductID);
                            command.Parameters.AddWithValue("@ProductName", cartItem.ProductName);
                            command.Parameters.AddWithValue("@ProductQuantity", cartItem.ProductQuantity);
                            command.Parameters.AddWithValue("@ProductPrice", cartItem.ProductPrice);
                            command.Parameters.AddWithValue("@ProductSubTotal", cartItem.ProductSubTotal);
                            command.Parameters.AddWithValue("@orderTotal", orderTotal); // Use the calculated order total
                            command.Parameters.AddWithValue("@UserName", currentUser.UserName);
                            command.Parameters.AddWithValue("@OrderStatus", "Order Placed");
                            command.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    // Set IDENTITY_INSERT OFF
                    string setIdentityInsertOff = "SET IDENTITY_INSERT OrderDetails OFF";
                    using (SqlCommand cmd = new SqlCommand(setIdentityInsertOff, connection))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }

                    SuccessMessage = "Items have been successfully added to your order!";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred while inserting cart items into order: " + ex.Message);
                throw; // Rethrow the exception to propagate it to the caller
            }
        }
    }
}
