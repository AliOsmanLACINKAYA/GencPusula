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
        public string? SeciliTur { get; set; }

        public List<string> Kategoriler { get; set; } = new()
        {
            "Burs", "Staj", "Etkinlik", "Yarışma", "Eğitim"
        };

        public async Task OnGetAsync()
        {
            var sorgu = _db.Firsatlar.AsQueryable();

            if (!string.IsNullOrWhiteSpace(AramaMetni))
            {
                sorgu = sorgu.Where(f =>
                    f.Baslik.Contains(AramaMetni) ||
                    f.Aciklama.Contains(AramaMetni) ||
                    f.Sehir.Contains(AramaMetni));
            }

            if (!string.IsNullOrWhiteSpace(SeciliTur))
            {
                sorgu = sorgu.Where(f => f.FirsatTuru == SeciliTur);
            }

            FirsatListesi = await sorgu.OrderBy(f => f.SonBasvuruTarihi).ToListAsync();
        }
    }
}