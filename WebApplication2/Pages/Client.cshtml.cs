using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KhumaloCraft2.Pages
{
	[Authorize(Roles = "client")]
	public class ClientsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
