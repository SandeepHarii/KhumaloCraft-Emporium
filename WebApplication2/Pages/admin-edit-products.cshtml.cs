using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace KhumaloCraft2.Pages
{
    public class EditModel : PageModel
    {
        public Product EditProduct { get; set; } = new Product();

        public string SuccessMessage { get; set; } = "Product Updated Successfully!";

        public bool EditAttempted { get; set; }
        public string ErrorMessage = "Failed to Update Product!";

       
        public class Product
        {
            public int ProductID { get; set; }
            public string ProductName { get; set; }
            public string ProductDescription { get; set; }
            public string ProductPrice { get; set; }
            public string ProductCategory { get; set; }
            public string ProductAvailability { get; set; }
        }

        public void OnGet()
        {
            try
            {
                string connectionString = "Server=tcp:st10050442-part2.database.windows.net,1433;Initial Catalog=KhumaloCraft_DB;Persist Security Info=False;User ID=Sandeep;Password=H@numan108;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT [productID], [productName], [productDescription], CAST([productPrice] AS varchar) as ProductPrice, [productCategory], CAST([productAvailability] AS varchar) as Availability from [dbo].[Products] WHERE ProductID = @productID";

                    string productID = Request.Query["id"];

                    Console.WriteLine();
                    Console.WriteLine("***************************************************************");
                    Console.WriteLine();
                    Console.WriteLine("Executing SELECT FROM PRODUCTS BY PRODUCT ID: ");
                    Console.WriteLine(sql);
                    Console.WriteLine($"Values: ProductID = {productID}");
                    Console.WriteLine();
                    Console.WriteLine("***************************************************************");
                    Console.WriteLine();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@productID", productID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                EditProduct.ProductID = reader.GetInt32(0);
                                EditProduct.ProductName = reader.GetString(1);
                                EditProduct.ProductDescription = reader.GetString(2);
                                EditProduct.ProductPrice = reader.GetString(3);
                                EditProduct.ProductCategory = reader.GetString(4);
                                EditProduct.ProductAvailability = reader.GetString(5);

                                Console.WriteLine($"ProductPrice: {EditProduct.ProductPrice}");
                                Console.WriteLine($"ProductCategory: {EditProduct.ProductCategory}");
                                Console.WriteLine($"ProductAvailability: {EditProduct.ProductAvailability}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public IActionResult OnPost()
        {
            string productID = Request.Query["id"];
            EditProduct.ProductName = Request.Form["productName"];
            EditProduct.ProductPrice = Request.Form["productPrice"];
            EditProduct.ProductDescription = Request.Form["productDescription"];
            EditProduct.ProductCategory = Request.Form["productCategory"];
            EditProduct.ProductAvailability = Request.Form["productAvailability"];

            if (string.IsNullOrEmpty(productID))
            {
                ModelState.AddModelError("EditProduct.ProductID", "Product ID is required.");
            }

            if (string.IsNullOrEmpty(EditProduct.ProductName) || string.IsNullOrEmpty(EditProduct.ProductDescription) || string.IsNullOrEmpty(EditProduct.ProductCategory))
            {
                ModelState.AddModelError("", "All Fields Are Required");
            }

            if (!ModelState.IsValid)
            {
                // If there are validation errors, return the page with validation errors
                return Page();
            }

            try
            {
                string connectionString = "Server=tcp:st10050442-part2.database.windows.net,1433;Initial Catalog=KhumaloCraft_DB;Persist Security Info=False;User ID=Sandeep;Password=H@numan108;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "UPDATE Products SET ProductName = @productName, ProductDescription = @productDescription, ProductPrice = @productPrice, ProductCategory = @productCategory, ProductAvailability = @productAvailability WHERE ProductID = @productID";

                    Console.WriteLine();
                    Console.WriteLine("***************************************************************");
                    Console.WriteLine();
                    Console.WriteLine("Executing UPDATED PRODUCTS BY PRODUCT ID: ");
                    Console.WriteLine();
                    Console.WriteLine(sql);
                    Console.WriteLine($"Values: ProductID = {EditProduct.ProductID}, ProductName = {EditProduct.ProductName}, ProductDescription = {EditProduct.ProductDescription}, ProductPrice = {EditProduct.ProductPrice}, ProductCategory = {EditProduct.ProductCategory}, ProductAvailability = {EditProduct.ProductAvailability}");
                    Console.WriteLine();
                    Console.WriteLine("***************************************************************");
                    Console.WriteLine();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@productName", EditProduct.ProductName);
                        command.Parameters.AddWithValue("@productDescription", EditProduct.ProductDescription);
                        command.Parameters.AddWithValue("@productPrice", EditProduct.ProductPrice);
                        command.Parameters.AddWithValue("@productCategory", EditProduct.ProductCategory);
                        command.Parameters.AddWithValue("@productAvailability", EditProduct.ProductAvailability);
                        command.Parameters.AddWithValue("@productID", productID);

                        // Execute the update operation
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Update successful, set success message
                            SuccessMessage = "Product Updated Successfully!";
                        }
                        else
                        {
                            // No rows affected, set error message
                            ErrorMessage = "Failed to Update Product!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            EditAttempted = true;
            return Page();
        }


    }
}