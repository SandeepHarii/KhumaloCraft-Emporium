using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace KhumaloCraft2.Pages
{
    public class EditCartItemsModel : PageModel
    {
        public Product product = new Product();
        public string ErrorMessage;
        public string successMessage;

        public void OnGet(string cartID) // Receive CartID parameter from URL
        {
            if (string.IsNullOrEmpty(cartID))
            {
                ErrorMessage = "Cart ID is required.";
                return;
            }

            try
            {
                string connectionString = "Server=tcp:st10050442-part2.database.windows.net,1433;Initial Catalog=KhumaloCraft_DB;Persist Security Info=False;User ID=Sandeep;Password=H@numan108;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT ProductQuantity FROM Cart WHERE CartID = @cartID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@cartID", cartID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                product.ProductQuantity = reader.GetInt32(0);
                            }
                            else
                            {
                                ErrorMessage = "Cart item not found.";
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

        public void OnPost(string cartID) // Receive CartID parameter from URL
        {
            if (string.IsNullOrEmpty(cartID))
            {
                ErrorMessage = "Invalid Cart ID";
                return;
            }

            if (!int.TryParse(Request.Form["productQuantity"], out int quantity) || quantity < 0)
            {
                ErrorMessage = "Quantity cannot be less than 0";
                return;
            }

            try
            {
                string connectionString = "Server=tcp:st10050442-part2.database.windows.net,1433;Initial Catalog=KhumaloCraft_DB;Persist Security Info=False;User ID=Sandeep;Password=H@numan108;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "UPDATE Cart SET ProductQuantity = @productQuantity WHERE CartID = @cartID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@productQuantity", quantity);
                        command.Parameters.AddWithValue("@cartID", cartID);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            successMessage = "Quantity updated successfully";
                        }
                        else
                        {
                            ErrorMessage = "Cart item not found or quantity not changed.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Response.Redirect("/Error"); // Redirect to error page
                return;
            }

            Response.Redirect("/Cart"); // Redirect back to Cart page
        }
    }

}
