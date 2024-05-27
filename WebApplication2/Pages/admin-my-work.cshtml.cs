using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace KhumaloCraft2.Pages
{
    public class AdminMyWorkModel : PageModel
    {
        public List<Product> ListProducts { get; private set; }

        public AdminMyWorkModel()
        {
            ListProducts = new List<Product>();
        }

        public void OnGet()
        {
            try
            {
                string connectionString = "Server=tcp:st10050442-part2.database.windows.net,1433;Initial Catalog=KhumaloCraft_DB;Persist Security Info=False;User ID=Sandeep;Password=H@numan108;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM Products";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Product product = new Product
                                {
                                    ProductID = reader.GetInt32(0),
                                   Image = reader.IsDBNull(1) ? null : (byte[])reader["Image"],
                                    ProductName = reader.GetString(2),
                                    ProductDescription = reader.GetString(3),
                                    ProductPrice = reader.GetDecimal(4),
                                    ProductCategory = reader.GetString(5),
                                    ProductAvailability = reader.GetInt32(6)

                                };

                                ListProducts.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately (logging, displaying user-friendly message, etc.)
                Console.WriteLine("Exception occurred while retrieving product data: " + ex.Message);
                throw; // Rethrow the exception to halt execution and view the stack trace
            }
        }

    }

}
