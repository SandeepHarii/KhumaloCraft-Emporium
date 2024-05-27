using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace KhumaloCraft2.Pages
{
    public class AdminEditOrdersModel : PageModel
    {
        private readonly string _connectionString = "Server=tcp:st10050442-part2.database.windows.net,1433;Initial Catalog=KhumaloCraft_DB;Persist Security Info=False;User ID=Sandeep;Password=H@numan108;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        [BindProperty(SupportsGet = true)]
        public int OrderId { get; set; }

        [BindProperty]
        public string ShipmentStatus { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (OrderId <= 0)
            {
                ErrorMessage = "Invalid Order ID.";
                return Page();
            }

            await LoadCurrentStatusAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(ShipmentStatus))
            {
                ErrorMessage = "Please select a shipment status.";
                await LoadCurrentStatusAsync();
                return Page();
            }

            await UpdateOrderStatusAsync();
            SuccessMessage = "Shipment Status Updated Successfully!";
            return Page();
        }

        private async Task LoadCurrentStatusAsync()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "SELECT OrderStatus FROM OrderDetails WHERE OrderID = @OrderId";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", OrderId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                ShipmentStatus = reader.GetString(0);
                            }
                            else
                            {
                                ErrorMessage = "Order not found.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error loading current status: " + ex.Message;
            }
        }

        private async Task UpdateOrderStatusAsync()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "UPDATE OrderDetails SET OrderStatus = @OrderStatus WHERE OrderID = @OrderId";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@OrderStatus", ShipmentStatus);
                        command.Parameters.AddWithValue("@OrderId", OrderId);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error updating order status: " + ex.Message;
            }
        }
    }
}
