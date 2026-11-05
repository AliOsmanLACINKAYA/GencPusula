using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GencPusula.Web.Data;
using GencPusula.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GencPusula.Web.Pages.Yonetim
{
    [Authorize(Roles = Roller.Yonetici)]
    public class FirsatlarModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public FirsatlarModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<Firsat> Firsatlar { get; set; } = new();

        public async Task OnGetAsync()
        {
            Firsatlar = await _db.Firsatlar
                .AsNoTracking()
                .OrderBy(f => f.SonBasvuruTarihi)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostSilAsync(int id)
        {
            var firsat = await _db.Firsatlar.FindAsync(id);
            if (firsat != null)
            {
                _db.Firsatlar.Remove(firsat);
                await _db.SaveChangesAsync();
                TempData["BasariMesaji"] = "Fırsat ilanı başarıyla silindi.";
            }

            return RedirectToPage();
        }
    }
}