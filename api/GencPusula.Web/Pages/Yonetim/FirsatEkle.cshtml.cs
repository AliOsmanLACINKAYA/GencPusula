using System;
using System.Threading.Tasks;
using GencPusula.Web.Data;
using GencPusula.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GencPusula.Web.Pages.Yonetim
{
    [Authorize(Roles = Roller.Yonetici)]
    public class FirsatEkleModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public FirsatEkleModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Firsat Firsat { get; set; } = new()
        {
            SonBasvuruTarihi = DateTime.UtcNow.Date.AddDays(14)
        };

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Firsat.SonBasvuruTarihi = DateTime.SpecifyKind(Firsat.SonBasvuruTarihi, DateTimeKind.Utc);

            _db.Firsatlar.Add(Firsat);
            await _db.SaveChangesAsync();

            TempData["BasariMesaji"] = "Yeni fırsat ilanı başarıyla eklendi.";
            return RedirectToPage("/Yonetim/Firsatlar");
        }
    }
}