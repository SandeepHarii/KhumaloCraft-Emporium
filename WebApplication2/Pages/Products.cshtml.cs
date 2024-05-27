using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using KhumaloCraft2.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace KhumaloCraft2.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _connectionString;

        public List<Product> ListProducts { get; set; }

        public ProductsModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _connectionString = "Server=tcp:st10050442-part2.database.windows.net,1433;Initial Catalog=KhumaloCraft_DB;Persist Security Info=False;User ID=Sandeep;Password=H@numan108;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ListProducts = await FetchProductsFromDatabaseAsync();
            return Page();
        }

        public async Task<IActionResult> OnGetAddToCartAsync(int productId, int quantity)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = "/Products" });
            }

            Product product = await FetchProductFromDatabaseAsync(productId);
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);

            if (product != null)
            {
                decimal subtotal = product.ProductPrice * quantity;

                // Update product availability
                int updatedAvailability = product.ProductAvailability - quantity;
                if (updatedAvailability < 0)
                {
                    // Handle insufficient availability scenario, you may add your own logic here
                    // For simplicity, we'll set availability to 0
                    updatedAvailability = 0;
                }

                // Save updated availability to database
                await UpdateProductAvailabilityInDatabaseAsync(productId, updatedAvailability);

                // Create cart item
                CartItem cartItem = new CartItem
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    ProductQuantity = quantity,
                    ProductPrice = product.ProductPrice,
                    ProductSubTotal = subtotal,
                    DateAdded = DateTime.Now,
                    UserName = currentUser.UserName // Assuming UserName is derived from User object
                };

                // Save cart item to database
                await SaveCartItemToDatabase(cartItem);
            }

            return RedirectToPage("/Cart");
        }

        private async Task<List<Product>> FetchProductsFromDatabaseAsync()
        {
            List<Product> products = new List<Product>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "SELECT P.ProductID, PH.Image, ProductName, ProductDescription, ProductCategory, ProductAvailability, ProductPrice\r\nFROM [dbo].[Products]  P\r\nLEFT JOIN [dbo].[Photos] PH\r\nON P.[ProductID] = PH.[productID] \r\nWhere  PH.[image] IS NOT  NULL";


                    Console.WriteLine();
                    Console.WriteLine("***************************************************************");
                    Console.WriteLine();
                    Console.WriteLine("Executing INSERT into ORDER DETAILS: ");
                    Console.WriteLine(sql);
                   /* Console.WriteLine($"Values: ProductID={products.ProductID}, ProductName={.ProductName}, ProductQuantity={cartItem.ProductQuantity}, " +
                                      $"ProductPrice={cartItem.ProductPrice}, ProductSubTotal={cartItem.ProductSubTotal}, orderTotal={orderTotal}, UserName={cartItem.UserName}, OrderStatus='Order Placed', " +
                                      $"OrderDate={DateTime.Now}");*/
                    Console.WriteLine();
                    Console.WriteLine("***************************************************************");
                    Console.WriteLine();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Product product = new Product
                                {
                                    ProductID = reader.GetInt32(0),
                                    Image = reader.IsDBNull(1) ? null : (byte[])reader["Image"],
                                    ProductName = reader.GetString(2),
                                    ProductDescription = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    ProductCategory = reader.GetString(4),
                                    ProductAvailability = reader.GetInt32(5),
                                    ProductPrice = reader.GetDecimal(6)
                                };

                                products.Add(product);
                            }



                        }                 
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred while fetching products from database: " + ex.Message);
            }

            return products;
        }

        private async Task<Product> FetchProductFromDatabaseAsync(int productId)
        {
            Product product = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "SELECT ProductName, ProductPrice, ProductAvailability FROM Products WHERE ProductID = @ProductId";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ProductId", productId);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                product = new Product
                                {
                                    ProductID = productId,
                                    ProductName = reader.GetString(0),
                                    ProductPrice = reader.GetDecimal(1),
                                    ProductAvailability = reader.GetInt32(2)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred while fetching product from database: " + ex.Message);
            }

            return product;
        }

        private async Task SaveCartItemToDatabase(CartItem cartItem)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "INSERT INTO Cart (ProductID, ProductName, ProductQuantity, ProductPrice, ProductSubTotal, UserName, DateAdded) " +
                                 "VALUES (@ProductID, @ProductName, @ProductQuantity, @ProductPrice, @ProductSubTotal, @UserName, @DateAdded)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ProductID", cartItem.ProductID);
                        command.Parameters.AddWithValue("@ProductName", cartItem.ProductName);
                        command.Parameters.AddWithValue("@ProductQuantity", cartItem.ProductQuantity);
                        command.Parameters.AddWithValue("@ProductPrice", cartItem.ProductPrice);
                        command.Parameters.AddWithValue("@ProductSubTotal", cartItem.ProductSubTotal);
                        command.Parameters.AddWithValue("@UserName", cartItem.UserName);
                        command.Parameters.AddWithValue("@DateAdded", cartItem.DateAdded);
                        await command.ExecuteNonQueryAsync();
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred while saving cart item to database: " + ex.Message);
            }
        }

        private async Task UpdateProductAvailabilityInDatabaseAsync(int productId, int availability)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "UPDATE Products SET ProductAvailability = @Availability WHERE ProductID = @ProductId";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Availability", availability);
                        command.Parameters.AddWithValue("@ProductId", productId);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred while updating product availability in database: " + ex.Message);
            }
        }
    }


    public class CartItem
    {
        public int OrderID { get; set; }
        public int CartID { get; set; }
        public int ProductID { get; set; }
        public byte[] Image { get; set; }
        public string ProductName { get; set; }
        public int ProductQuantity { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ProductSubTotal { get; set; }
        public decimal orderTotal { get; set; }
        public string UserName { get; set; }

        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
