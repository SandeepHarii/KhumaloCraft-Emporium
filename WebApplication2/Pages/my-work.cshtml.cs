using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;

namespace KhumaloCraft2.Pages
{
    public class MyWorkModel : PageModel
    {
        public List<Product> ListProducts { get; private set; }

        public MyWorkModel()
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
                                    ProductName = reader.GetString(1),
                                    ProductDescription = reader.GetString(2),
                                    ProductPrice = reader.GetDecimal(3),
                                    ProductCategory = reader.GetString(4),
                                    ProductAvailability = reader.GetInt32(5)

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

    public class Product
    {
        public int OrderID { get; set; }
        public int CartID { get; set; }
        public int ProductID { get; set; }
        public byte[] Image { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductQuantity { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }

        public DateTime OrderDate { get; set; }
        public string ProductCategory { get; set; }
        public int ProductAvailability { get; set; }
    }


}


