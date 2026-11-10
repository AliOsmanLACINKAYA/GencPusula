using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using GencPusula.Web.Data;
using GencPusula.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GencPusula.Web.Controllers.Api;

[ApiController]
[Route("api/kayitli-firsatlar")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class KayitliFirsatlarController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public KayitliFirsatlarController(ApplicationDbContext db)
    {
        _db = db;
    }

    private string KullaniciId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")
        ?? string.Empty;

    private Task<int?> ProfilIdGetirAsync() =>
        _db.KullaniciProfilleri
            .Where(p => p.KullaniciId == KullaniciId)
            .Select(p => (int?)p.Id)
            .FirstOrDefaultAsync();

    [HttpGet]
    public async Task<IActionResult> Listele()
    {
        var profilId = await ProfilIdGetirAsync();
        if (profilId is null)
            return Ok(new List<KayitliFirsatDto>());

        var kayitlar = await _db.BasvuruKayitlari
            .AsNoTracking()
            .Include(b => b.Firsat)
            .Where(b => b.KullaniciProfiliId == profilId)
            .OrderByDescending(b => b.KayitTarihi)
            .ToListAsync();

        return Ok(kayitlar
            .Select(k => KayitliFirsatDto.Olustur(k, k.Firsat!))
            .ToList());
    }

    [HttpPost]
    public async Task<IActionResult> Ekle(KayitliFirsatIstegi istek)
    {
        var firsat = await _db.Firsatlar
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == istek.FirsatId);

        if (firsat is null)
        {
            return NotFound(new
            {
                kod = "Bulunamadi",
                mesaj = "Fırsat bulunamadı."
            });
        }

        var profilId = await ProfilIdGetirAsync();
        if (profilId is null)
        {
            return Conflict(new
            {
                kod = "ProfilGerekli",
                mesaj = "Önce profil oluşturmalısınız."
            });
        }

        var zatenKayitli = await _db.BasvuruKayitlari
            .AnyAsync(b => b.KullaniciProfiliId == profilId && b.FirsatId == istek.FirsatId);

        if (zatenKayitli)
        {
            return Conflict(new
            {
                kod = "ZatenKayitli",
                mesaj = "Bu fırsat zaten kayıtlı."
            });
        }

        var kayit = new BasvuruKaydi
        {
            KullaniciProfiliId = profilId.Value,
            FirsatId = istek.FirsatId
        };

        _db.BasvuruKayitlari.Add(kayit);
        await _db.SaveChangesAsync();

        return StatusCode(201, KayitliFirsatDto.Olustur(kayit, firsat));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Sil(int id)
    {
        var profilId = await ProfilIdGetirAsync();

        var kayit = profilId is null
            ? null
            : await _db.BasvuruKayitlari
                .FirstOrDefaultAsync(b => b.Id == id && b.KullaniciProfiliId == profilId);

        if (kayit is null)
        {
            return NotFound(new
            {
                kod = "Bulunamadi",
                mesaj = "Kayıt bulunamadı."
            });
        }

        _db.BasvuruKayitlari.Remove(kayit);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}

public class KayitliFirsatIstegi
{
    [Range(1, int.MaxValue, ErrorMessage = "Fırsat seçilmelidir.")]
    public int FirsatId { get; set; }
}

public class KayitliFirsatDto
{
    public int Id { get; set; }
    public int FirsatId { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string FirsatTuru { get; set; } = string.Empty;
    public string Sehir { get; set; } = string.Empty;
    public DateTime SonBasvuruTarihi { get; set; }
    public string GuncellikEtiketi { get; set; } = string.Empty;
    public string BasvuruDurumu { get; set; } = string.Empty;
    public string? KisiselNotlar { get; set; }
    public DateTime KayitTarihi { get; set; }

    public static KayitliFirsatDto Olustur(BasvuruKaydi kayit, Firsat firsat)
    {
        return new KayitliFirsatDto
        {
            Id = kayit.Id,
            FirsatId = kayit.FirsatId,
            Baslik = firsat.Baslik,
            FirsatTuru = firsat.FirsatTuru,
            Sehir = firsat.Sehir,
            SonBasvuruTarihi = firsat.SonBasvuruTarihi,
            GuncellikEtiketi = GuncellikDurumu.Hesapla(firsat.SonKontrolTarihi).Etiket,
            BasvuruDurumu = kayit.BasvuruDurumu,
            KisiselNotlar = kayit.KisiselNotlar,
            KayitTarihi = kayit.KayitTarihi
        };
    }
}
