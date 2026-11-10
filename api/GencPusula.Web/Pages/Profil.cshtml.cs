using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GencPusula.Web.Data;
using GencPusula.Web.Models;
using GencPusula.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GencPusula.Web.Pages
{
    [Authorize]
    public class ProfilModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProfilModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public KullaniciProfili Profil { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public bool Duzenle { get; set; }

        public bool KayitliProfilVarMi { get; set; }

        public List<string> EksikAlanlar => ProfilKurallari.EksikAlanlar(Profil);

        public List<string> SinifSecenekleri { get; set; } = new()
        {
            "Hazırlık",
            "1. Sınıf",
            "2. Sınıf",
            "3. Sınıf",
            "4. Sınıf",
            "5. Sınıf",
            "6. Sınıf",
            "Yüksek Lisans",
            "Doktora",
            "Mezun"
        };

        public List<string> DilSeviyesiSecenekleri { get; set; } = new()
        {
            "Belirtilmedi",
            "A1 - Başlangıç",
            "A2 - Temel",
            "B1 - Orta Altı",
            "B2 - Orta Üstü",
            "C1 - İleri",
            "C2 - Yetkin"
        };

        private string AktifKullaniciIdGetir()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }

        public async Task OnGetAsync()
        {
            var kullaniciId = AktifKullaniciIdGetir();
            var mevcutProfil = await _context.Set<KullaniciProfili>()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.KullaniciId == kullaniciId);

            if (mevcutProfil != null)
            {
                Profil = mevcutProfil;
                KayitliProfilVarMi = true;
            }
            else
            {
                Profil = new KullaniciProfili
                {
                    KullaniciId = kullaniciId
                };
                KayitliProfilVarMi = false;
                Duzenle = true;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var kullaniciId = AktifKullaniciIdGetir();
            Profil.KullaniciId = kullaniciId;
            ModelState.Remove("Profil.KullaniciId");

            if (!ModelState.IsValid)
            {
                KayitliProfilVarMi = await _context.Set<KullaniciProfili>()
                    .AnyAsync(p => p.KullaniciId == kullaniciId);
                Duzenle = true;
                return Page();
            }

            Profil.IlgiAlanlari ??= string.Empty;
            Profil.TercihEdilenTurler ??= string.Empty;
            Profil.Sehir ??= string.Empty;

            var mevcutProfil = await _context.Set<KullaniciProfili>()
                .FirstOrDefaultAsync(p => p.KullaniciId == kullaniciId);

            if (mevcutProfil == null)
            {
                Profil.Id = 0;
                Profil.GuncellenmeTarihi = DateTime.UtcNow;
                _context.Set<KullaniciProfili>().Add(Profil);
                TempData["BasariMesaji"] = "Profiliniz başarıyla oluşturuldu.";
            }
            else
            {
                mevcutProfil.AdSoyad = Profil.AdSoyad;
                //mevcutProfil.Universite = Profil.Universite;
                mevcutProfil.Bolum = Profil.Bolum;
                mevcutProfil.Sinif = Profil.Sinif;
                mevcutProfil.IlgiAlanlari = Profil.IlgiAlanlari;
                mevcutProfil.TercihEdilenTurler = Profil.TercihEdilenTurler;
                mevcutProfil.Sehir = Profil.Sehir;
                mevcutProfil.GenelNotOrtalamasi = Profil.GenelNotOrtalamasi;
                mevcutProfil.YabanciDilSeviyesi = Profil.YabanciDilSeviyesi;
                mevcutProfil.GuncellenmeTarihi = DateTime.UtcNow;
                TempData["BasariMesaji"] = "Profil bilgileriniz başarıyla güncellendi.";
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("/Profil");
        }
    }
}