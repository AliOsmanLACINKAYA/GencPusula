using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using GencPusula.Web.Data;
using GencPusula.Web.Models;
using GencPusula.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GencPusula.Web.Controllers.Api;

[ApiController]
[Route("api/profil")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class ProfilController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ProfilController(ApplicationDbContext db)
    {
        _db = db;
    }

    private string KullaniciId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")
        ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> Getir()
    {
        var profil = await _db.KullaniciProfilleri
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.KullaniciId == KullaniciId);

        return Ok(ProfilDto.Olustur(profil));
    }

    [HttpPut]
    public async Task<IActionResult> Guncelle(ProfilIstegi istek)
    {
        var profil = await _db.KullaniciProfilleri
            .FirstOrDefaultAsync(p => p.KullaniciId == KullaniciId);

        if (profil is null)
        {
            profil = new KullaniciProfili { KullaniciId = KullaniciId };
            _db.KullaniciProfilleri.Add(profil);
        }

        profil.AdSoyad = istek.AdSoyad;
        profil.Bolum = istek.Bolum;
        profil.Sinif = istek.Sinif;
        profil.IlgiAlanlari = istek.IlgiAlanlari ?? string.Empty;
        profil.TercihEdilenTurler = istek.TercihEdilenTurler ?? string.Empty;
        profil.Sehir = istek.Sehir ?? string.Empty;
        profil.GenelNotOrtalamasi = istek.GenelNotOrtalamasi;
        profil.YabanciDilSeviyesi = istek.YabanciDilSeviyesi;
        profil.GuncellenmeTarihi = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(ProfilDto.Olustur(profil));
    }
}

public class ProfilIstegi
{
    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    [StringLength(100, ErrorMessage = "Ad soyad en fazla 100 karakter olabilir.")]
    public string AdSoyad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bölüm zorunludur.")]
    [StringLength(150, ErrorMessage = "Bölüm en fazla 150 karakter olabilir.")]
    public string Bolum { get; set; } = string.Empty;

    [Required(ErrorMessage = "Sınıf zorunludur.")]
    [StringLength(50, ErrorMessage = "Sınıf en fazla 50 karakter olabilir.")]
    public string Sinif { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "İlgi alanları en fazla 500 karakter olabilir.")]
    public string? IlgiAlanlari { get; set; }

    [StringLength(200, ErrorMessage = "Tercih edilen türler en fazla 200 karakter olabilir.")]
    public string? TercihEdilenTurler { get; set; }

    [StringLength(100, ErrorMessage = "Şehir en fazla 100 karakter olabilir.")]
    public string? Sehir { get; set; }

    [Range(0.00, 4.00, ErrorMessage = "Genel not ortalaması 0 ile 4 arasında olmalıdır.")]
    public decimal? GenelNotOrtalamasi { get; set; }

    [StringLength(50, ErrorMessage = "Yabancı dil seviyesi en fazla 50 karakter olabilir.")]
    public string? YabanciDilSeviyesi { get; set; }
}

public class ProfilDto
{
    public bool ProfilVar { get; set; }
    public string AdSoyad { get; set; } = string.Empty;
    public string Bolum { get; set; } = string.Empty;
    public string Sinif { get; set; } = string.Empty;
    public string IlgiAlanlari { get; set; } = string.Empty;
    public string TercihEdilenTurler { get; set; } = string.Empty;
    public string Sehir { get; set; } = string.Empty;
    public decimal? GenelNotOrtalamasi { get; set; }
    public string? YabanciDilSeviyesi { get; set; }
    public DateTime? GuncellenmeTarihi { get; set; }
    public bool ProfilTamamlandi { get; set; }
    public List<string> EksikAlanlar { get; set; } = new();

    public static ProfilDto Olustur(KullaniciProfili? profil)
    {
        if (profil is null)
        {
            return new ProfilDto
            {
                ProfilVar = false,
                EksikAlanlar = ProfilKurallari.EksikAlanlar(null)
            };
        }

        return new ProfilDto
        {
            ProfilVar = true,
            AdSoyad = profil.AdSoyad,
            Bolum = profil.Bolum,
            Sinif = profil.Sinif,
            IlgiAlanlari = profil.IlgiAlanlari ?? string.Empty,
            TercihEdilenTurler = profil.TercihEdilenTurler ?? string.Empty,
            Sehir = profil.Sehir ?? string.Empty,
            GenelNotOrtalamasi = profil.GenelNotOrtalamasi,
            YabanciDilSeviyesi = profil.YabanciDilSeviyesi,
            GuncellenmeTarihi = profil.GuncellenmeTarihi,
            ProfilTamamlandi = ProfilKurallari.Tamamlandi(profil),
            EksikAlanlar = ProfilKurallari.EksikAlanlar(profil)
        };
    }
}
