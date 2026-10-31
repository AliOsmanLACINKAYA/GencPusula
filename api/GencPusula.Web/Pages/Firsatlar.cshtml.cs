using GencPusula.Web.Data;
using GencPusula.Web.Models;
using Microsoft.AspNetCore.Mvc;
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

    public IReadOnlyList<string> Kategoriler { get; private set; } = new List<string>();

    [BindProperty(SupportsGet = true)]
    public string? Arama { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SeciliKategori { get; set; }

    public async Task OnGetAsync()
    {
        Kategoriler = await _db.Firsatlar
            .AsNoTracking()
            .Where(f => !string.IsNullOrEmpty(f.FirsatTuru))
            .Select(f => f.FirsatTuru)
            .Distinct()
            .OrderBy(k => k)
            .ToListAsync();

        var sorgu = _db.Firsatlar.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(SeciliKategori))
        {
            sorgu = sorgu.Where(f => f.FirsatTuru == SeciliKategori);
        }

        if (!string.IsNullOrWhiteSpace(Arama))
        {
            var aranan = Arama.Trim().ToLower();
            sorgu = sorgu.Where(f =>
                f.Baslik.ToLower().Contains(aranan) ||
                f.Aciklama.ToLower().Contains(aranan) ||
                f.Sehir.ToLower().Contains(aranan));
        }

        Firsatlar = await sorgu
            .OrderBy(f => f.SonBasvuruTarihi)
            .ToListAsync();
    }
}
