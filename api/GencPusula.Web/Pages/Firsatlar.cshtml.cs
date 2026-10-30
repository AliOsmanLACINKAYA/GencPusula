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
        private readonly ApplicationDbContext _context;

        public FirsatlarModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Firsat> Firsatlar { get; set; } = new List<Firsat>();

        public IList<string> Kategoriler { get; set; } = new List<string>();

        [BindProperty(SupportsGet = true)]
        public string? Arama { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SeciliKategori { get; set; }

        public async Task OnGetAsync()
        {
            Kategoriler = await _context.Firsatlar
                .Where(f => !string.IsNullOrEmpty(f.FirsatTuru))
                .Select(f => f.FirsatTuru)
                .Distinct()
                .OrderBy(k => k)
                .ToListAsync();

            var sorgu = _context.Firsatlar.AsQueryable();

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
}