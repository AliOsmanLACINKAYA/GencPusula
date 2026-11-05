using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GencPusula.Web.Data;
using GencPusula.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GencPusula.Web.Pages
{
    public class FirsatlarModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public FirsatlarModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<Firsat> FirsatListesi { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? AramaMetni { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SeciliKategori { get; set; }

        public List<string> Kategoriler { get; set; } = new();

        public async Task OnGetAsync()
        {
            Kategoriler = await KategoriListesi.YukleAsync(_db);

            var sorgu = _db.Firsatlar.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(AramaMetni))
            {
                var aranan = AramaMetni.Trim().ToLower();
                sorgu = sorgu.Where(f =>
                    f.Baslik.ToLower().Contains(aranan) ||
                    f.Aciklama.ToLower().Contains(aranan) ||
                    f.Sehir.ToLower().Contains(aranan));
            }

            if (!string.IsNullOrWhiteSpace(SeciliKategori))
            {
                sorgu = sorgu.Where(f => f.FirsatTuru == SeciliKategori);
            }

            FirsatListesi = await sorgu.OrderBy(f => f.SonBasvuruTarihi).ToListAsync();
        }
    }
}