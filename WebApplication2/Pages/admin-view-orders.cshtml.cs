using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using KhumaloCraft2.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace KhumaloCraft2.Pages
{
    public class AdminViewOrdersModel : PageModel
    {
        private readonly string _connectionString;
        private readonly UserManager<ApplicationUser> _userManager;

        public List<CartItem> CartItems { get; private set; }

        public AdminViewOrdersModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _connectionString = "Server=tcp:st10050442-part2.database.windows.net,1433;Initial Catalog=KhumaloCraft_DB;Persist Security Info=False;User ID=Sandeep;Password=H@numan108;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            CartItems = new List<CartItem>();
        }

        public async Task OnGet()
        {
            CartItems = await FetchCartItemFromDatabase();
        }

        private async Task<List<CartItem>> FetchCartItemFromDatabase()
        {
            List<CartItem> cartItems = new List<CartItem>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "SELECT orderID, productID, productName, productQuantity, productPrice, productSubTotal, orderTotal, UserName, OrderStatus, orderDate FROM OrderDetails ORDER BY orderDate DESC";



                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                CartItem cartItem = new CartItem
                                {
                                    OrderID = reader.GetInt32(0),
                                    ProductID = reader.GetInt32(1),
                                    ProductName = reader.GetString(2),
                                    ProductQuantity = reader.GetInt32(3),
                                    ProductPrice = reader.GetDecimal(4),
                                    ProductSubTotal = reader.GetDecimal(5),
                                    orderTotal = reader.GetDecimal(6),
                                    UserName = reader.GetString(7),
                                    OrderStatus = reader.GetString(8),
                                    OrderDate = reader.GetDateTime(9)
                                };

                                cartItems.Add(cartItem);
                            }
                        }
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