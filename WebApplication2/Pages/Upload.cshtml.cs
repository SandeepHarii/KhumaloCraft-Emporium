using KhumaloCraft2.Pages;
using KhumaloCraft2.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

public class UploadModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public UploadModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public IFormFile UploadFile { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (UploadFile != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await UploadFile.CopyToAsync(memoryStream);

                var photo = new Photo
                {
                    FileName = UploadFile.FileName,
                    Image = memoryStream.ToArray(),
                    UploadDate = DateTime.Now
                };

                _context.Photos.Add(photo);
                await _context.SaveChangesAsync();
            }
        }

        return RedirectToPage("./Gallery");
    }
}
