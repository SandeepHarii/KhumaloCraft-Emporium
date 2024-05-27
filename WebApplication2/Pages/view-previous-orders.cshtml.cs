using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using KhumaloCraft2.Models;
using Microsoft.AspNetCore.Identity;


namespace KhumaloCraft2.Pages
{
    public class ViewPreviousOrdersModel : PageModel
    {

        private readonly string _connectionString;
        private readonly UserManager<ApplicationUser> _userManager;
        public List<CartItem> CartItems { get; private set; }
        public string UserName { get; private set; }

        public ViewPreviousOrdersModel(UserManager<ApplicationUser> userManager)
        {
            
            _userManager = userManager;
            _connectionString = "Server=tcp:st10050442-part2.database.windows.net,1433;Initial Catalog=KhumaloCraft_DB;Persist Security Info=False;User ID=Sandeep;Password=H@numan108;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            CartItems = new List<CartItem>();
        }

        public async Task OnGet() // Changed return type to async Task
        {
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);
            UserName = currentUser.UserName; // Set the username here
            CartItems = await FetchCartItemFromDatabase(); // Await here since FetchCartItemFromDatabase is now async
        }

        private async Task<List<CartItem>> FetchCartItemFromDatabase() // Changed return type to async Task<List<CartItem>>
        {
            List<CartItem> cartItems = new List<CartItem>();

            try
            {
                ApplicationUser currentUser = await _userManager.GetUserAsync(User);

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(); // Use OpenAsync() for asynchronous opening of connection
                    string sql = "SELECT OD.orderID, P.productID, PH.Image, P.productName, OD.productQuantity, OD.productPrice, OD.productSubTotal, OD.orderTotal, OD.UserName, OD.OrderStatus, OD.orderDate FROM OrderDetails OD LEFT JOIN Products P ON OD.productID = P.productID LEFT JOIN Photos PH ON P.productID = PH.productID WHERE OD.UserName = @UserName ORDER BY OD.orderDate DESC";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserName", currentUser.UserName); // assuming UserName is the correct property
                        using (SqlDataReader reader = await command.ExecuteReaderAsync()) // Use ExecuteReaderAsync for asynchronous reading
                        {
                            while (await reader.ReadAsync()) // Use ReadAsync for asynchronous reading
                            {
                                CartItem cartItem = new CartItem
                                {
                                    OrderID = reader.GetInt32(0),
                                    ProductID = reader.GetInt32(1),
                                    Image = reader.IsDBNull(2) ? null : (byte[])reader["Image"],
                                    ProductName = reader.GetString(3),
                                    ProductQuantity = reader.GetInt32(4),
                                    ProductPrice = reader.GetDecimal(5),
                                    ProductSubTotal = reader.GetDecimal(6),
                                    orderTotal = reader.GetDecimal(7),
                                    UserName = reader.GetString(8),
                                    OrderStatus = reader.GetString(9),
                                    OrderDate = reader.GetDateTime(10)
                                };

                                cartItems.Add(cartItem);
                            }
                        }
                    }

                    // Delete cart details for the current user
                    string sqlClearCurrentUserCart = "DELETE FROM Cart WHERE UserName = @UserName";

                    Console.WriteLine();
                    Console.WriteLine("***************************************************************");
                    Console.WriteLine("Delete cart details for the current user ");
                    Console.WriteLine(sqlClearCurrentUserCart);
                    Console.WriteLine("***************************************************************");
                    Console.WriteLine();
                    using (SqlCommand commandTruncateCart = new SqlCommand(sqlClearCurrentUserCart, connection))
                    {
                        commandTruncateCart.Parameters.AddWithValue("@UserName", currentUser.UserName);
                        await commandTruncateCart.ExecuteNonQueryAsync();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred while fetching order details from database: " + ex.Message);
                // Consider logging the exception for better debugging.
            }

            return cartItems;
        }
    }
}