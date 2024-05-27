using KhumaloCraft2.Pages;
using KhumaloCraft2.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class GalleryModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public GalleryModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Photo> Photos { get; set; }

    public async Task OnGetAsync()
    {
        Photos = await _context.Photos.ToListAsync();
    }
}
