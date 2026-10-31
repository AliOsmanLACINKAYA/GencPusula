using GencPusula.Web.Data;
using GencPusula.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GencPusula.Web.Pages;

public class FirsatlarModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public FirsatlarModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public IReadOnlyList<Firsat> Firsatlar { get; private set; } = new List<Firsat>();

    public async Task OnGetAsync()
    {
        Firsatlar = await _db.Firsatlar
            .AsNoTracking()
            .OrderBy(f => f.SonBasvuruTarihi)
            .ToListAsync();
    }
}