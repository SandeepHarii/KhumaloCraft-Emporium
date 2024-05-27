using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace KhumaloCraft2.Pages
{
    public class AddNewProductModel : PageModel
    {
        public Product product = new Product();
        public string ErrorMessage = "";
        public string SuccessMessage = "";

        public class Product
        {
            public int ProductID { get; set; }
            public string ProductName { get; set; }
            public string ProductDescription { get; set; }
            public decimal ProductPrice { get; set; }
            public string ProductCategory { get; set; }
            public int ProductAvailability { get; set; }
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            product.ProductName = Request.Form["productName"];
            product.ProductDescription = Request.Form["productDescription"];

            // Convert product price to decimal
            if (!decimal.TryParse(Request.Form["productPrice"], out decimal productPrice))
            {
                ErrorMessage = "Invalid product price format";
                return;
            }
            product.ProductPrice = productPrice;

            // Convert product availability to integer
            if (!int.TryParse(Request.Form["productAvailability"], out int productAvailability))
            {
                ErrorMessage = "Invalid product availability format";
                return;
            }
            product.ProductAvailability = productAvailability;

            product.ProductCategory = Request.Form["productCategory"];

            // Validation: Check if any required fields are empty
            if (string.IsNullOrEmpty(product.ProductName) ||
                string.IsNullOrEmpty(product.ProductDescription) ||
                product.ProductPrice <= 0 ||
                string.IsNullOrEmpty(product.ProductCategory) ||
                product.ProductAvailability < 0) // Assuming availability cannot be negative
            {
                ErrorMessage = "All Fields Are Required";
                return;
            }

            try
            {
                string connectionString = "Server=tcp:st10050442-part2.database.windows.net,1433;Initial Catalog=KhumaloCraft_DB;Persist Security Info=False;User ID=Sandeep;Password=H@numan108;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "INSERT INTO Products (ProductName, ProductDescription, ProductPrice, ProductCategory, ProductAvailability) VALUES " +
                        "(@productName, @productDescription, @productPrice, @productCategory, @productAvailability)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@productName", product.ProductName);
                        command.Parameters.AddWithValue("@productDescription", product.ProductDescription);
                        command.Parameters.AddWithValue("@productPrice", product.ProductPrice);
                        command.Parameters.AddWithValue("@productCategory", product.ProductCategory);
                        command.Parameters.AddWithValue("@productAvailability", product.ProductAvailability);

                        command.ExecuteNonQuery();
                    }
                }

                // Clear product after successful insertion
                product.ProductName = "";
                product.ProductDescription = "";
                product.ProductPrice = 0;
                product.ProductCategory = "";
                product.ProductAvailability = 0;

                SuccessMessage = "New Product Added Successfully";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
