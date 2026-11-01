using GencPusula.Web.Data;
using GencPusula.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GencPusula.Web.Pages;

public class FirsatDetayModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public FirsatDetayModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public Firsat? Firsat { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Firsat = await _db.Firsatlar
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == id);

        if (Firsat is null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
        }

        return Page();
    }
}
