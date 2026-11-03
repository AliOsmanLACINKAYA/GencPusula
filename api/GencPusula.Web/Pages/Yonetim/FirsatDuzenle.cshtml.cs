using System;
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
    public class FirsatDuzenleModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public FirsatDuzenleModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Firsat Firsat { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var mevcutFirsat = await _db.Firsatlar.FirstOrDefaultAsync(f => f.Id == id);
            if (mevcutFirsat == null)
            {
                return RedirectToPage("/Yonetim/Firsatlar");
            }

            Firsat = mevcutFirsat;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var guncellenecekFirsat = await _db.Firsatlar.FirstOrDefaultAsync(f => f.Id == Firsat.Id);
            if (guncellenecekFirsat == null)
            {
                return RedirectToPage("/Yonetim/Firsatlar");
            }

            guncellenecekFirsat.Baslik = Firsat.Baslik;
            guncellenecekFirsat.FirsatTuru = Firsat.FirsatTuru;
            guncellenecekFirsat.Sehir = Firsat.Sehir;
            guncellenecekFirsat.SonBasvuruTarihi = DateTime.SpecifyKind(Firsat.SonBasvuruTarihi, DateTimeKind.Utc);
            guncellenecekFirsat.KaynakBaglantisi = Firsat.KaynakBaglantisi;
            guncellenecekFirsat.Aciklama = Firsat.Aciklama;

            await _db.SaveChangesAsync();

            TempData["BasariMesaji"] = "Fırsat ilanı başarıyla güncellendi.";
            return RedirectToPage("/Yonetim/Firsatlar");
        }
    }
}