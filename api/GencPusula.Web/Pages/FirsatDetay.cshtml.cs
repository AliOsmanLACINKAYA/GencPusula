using System.Security.Claims;
using System.Threading.Tasks;
using GencPusula.Web.Data;
using GencPusula.Web.Models;
using GencPusula.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GencPusula.Web.Pages
{
    public class FirsatDetayModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public FirsatDetayModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public Firsat? SeciliFirsat { get; set; }
        public UygunlukSonucu? UygunlukSonucu { get; set; }
        public bool ProfilKayitliMi { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            SeciliFirsat = await _db.Firsatlar
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);

            if (SeciliFirsat == null)
            {
                return RedirectToPage("/Firsatlar");
            }

            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            KullaniciProfili? profil = null;

            if (!string.IsNullOrEmpty(kullaniciId))
            {
                profil = await _db.Set<KullaniciProfili>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.KullaniciId == kullaniciId);
            }

            ProfilKayitliMi = profil != null;
            UygunlukSonucu = UygunlukKurallari.Degerlendir(SeciliFirsat, profil);

            return Page();
        }
    }
}