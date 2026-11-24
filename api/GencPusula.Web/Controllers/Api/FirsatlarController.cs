using System.Security.Claims;
using GencPusula.Web.Data;
using GencPusula.Web.Models;
using GencPusula.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GencPusula.Web.Controllers.Api;

[ApiController]
[Route("api/firsatlar")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class FirsatlarController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public FirsatlarController(ApplicationDbContext db)
    {
        _db = db;
    }

    private string KullaniciId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")
        ?? string.Empty;

    private Task<KullaniciProfili?> ProfilGetirAsync() =>
        _db.KullaniciProfilleri
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.KullaniciId == KullaniciId);

    [HttpGet]
    public async Task<IActionResult> Listele()
    {
        var firsatlar = await _db.Firsatlar
            .AsNoTracking()
            .OrderBy(f => f.SonBasvuruTarihi)
            .ToListAsync();

        var profil = await ProfilGetirAsync();

        return Ok(firsatlar.Select(f => FirsatDto.Olustur(f, profil)).ToList());
    }

    [HttpGet("ara")]
    public async Task<IActionResult> Ara(
        [FromQuery] string? kategori,
        [FromQuery] string? sehir,
        [FromQuery] string? arama,
        [FromQuery] int sayfa = 1,
        [FromQuery] int sayfaBoyutu = 10)
    {
        sayfa = Math.Max(1, sayfa);
        sayfaBoyutu = Math.Clamp(sayfaBoyutu, 1, 50);

        const string turkce = "ıİşŞğĞüÜöÖçÇ";
        const string ascii = "iissgguuoocc";

        var sorgu = _db.Firsatlar.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(kategori))
        {
            var kategoriAranan = $"%{kategori.Trim()}%";
            sorgu = sorgu.Where(f => EF.Functions.ILike(
                ApplicationDbContext.Translate(f.FirsatTuru, turkce, ascii),
                ApplicationDbContext.Translate(kategoriAranan, turkce, ascii)));
        }

        if (!string.IsNullOrWhiteSpace(sehir))
        {
            var sehirAranan = $"%{sehir.Trim()}%";
            sorgu = sorgu.Where(f => EF.Functions.ILike(
                ApplicationDbContext.Translate(f.Sehir, turkce, ascii),
                ApplicationDbContext.Translate(sehirAranan, turkce, ascii)));
        }

        if (!string.IsNullOrWhiteSpace(arama))
        {
            var aranan = arama.Trim().ToLower();
            sorgu = sorgu.Where(f =>
                f.Baslik.ToLower().Contains(aranan) ||
                f.Aciklama.ToLower().Contains(aranan) ||
                f.Sehir.ToLower().Contains(aranan));
        }

        var toplamKayit = await sorgu.CountAsync();
        var kayitlar = await sorgu
            .OrderBy(f => f.SonBasvuruTarihi)
            .Skip((sayfa - 1) * sayfaBoyutu)
            .Take(sayfaBoyutu)
            .ToListAsync();

        var profil = await ProfilGetirAsync();

        return Ok(new SayfaliFirsatCevabi
        {
            ToplamKayit = toplamKayit,
            Sayfa = sayfa,
            SayfaBoyutu = sayfaBoyutu,
            ToplamSayfa = (int)Math.Ceiling((double)toplamKayit / sayfaBoyutu),
            Kayitlar = kayitlar.Select(f => FirsatDto.Olustur(f, profil)).ToList()
        });
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = Roller.Yonetici)]
    public async Task<IActionResult> Ekle(Firsat istek)
    {
        if (!await _db.Kategoriler.AnyAsync(k => k.Ad == istek.FirsatTuru))
            ModelState.AddModelError(nameof(istek.FirsatTuru), "Geçerli bir kategori seçin.");

        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var firsat = new Firsat
        {
            FirsatTuru = istek.FirsatTuru,
            Baslik = istek.Baslik,
            Aciklama = istek.Aciklama,
            SonBasvuruTarihi = DateTime.SpecifyKind(istek.SonBasvuruTarihi, DateTimeKind.Utc),
            Sehir = istek.Sehir,
            KaynakBaglantisi = istek.KaynakBaglantisi,
            GerekliBolum = istek.GerekliBolum,
            GerekliSinif = istek.GerekliSinif,
            MinNotOrtalamasi = istek.MinNotOrtalamasi,
            GerekliDilSeviyesi = istek.GerekliDilSeviyesi,
            SonKontrolTarihi = DateTime.UtcNow
        };

        _db.Firsatlar.Add(firsat);
        await _db.SaveChangesAsync();

        return StatusCode(201, FirsatDto.Olustur(firsat, null));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detay(int id)
    {
        var firsat = await _db.Firsatlar
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == id);

        if (firsat is null)
        {
            return NotFound(new
            {
                kod = "Bulunamadi",
                mesaj = "Fırsat bulunamadı."
            });
        }

        var profil = await ProfilGetirAsync();

        return Ok(FirsatDto.Olustur(firsat, profil));
    }
}

public class FirsatDto
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
    public string FirsatTuru { get; set; } = string.Empty;
    public string Sehir { get; set; } = string.Empty;
    public DateTime SonBasvuruTarihi { get; set; }
    public DateTime? SonKontrolTarihi { get; set; }
    public string GuncellikEtiketi { get; set; } = string.Empty;
    public string KaynakBaglantisi { get; set; } = string.Empty;
    public string OnayDurumu { get; set; } = string.Empty;
    public string? GerekliBolum { get; set; }
    public string? GerekliSinif { get; set; }
    public decimal? MinNotOrtalamasi { get; set; }
    public string? GerekliDilSeviyesi { get; set; }
    public UygunlukDto Uygunluk { get; set; } = new();

    public static FirsatDto Olustur(Firsat firsat, KullaniciProfili? profil)
    {
        var sonuc = UygunlukKurallari.Degerlendir(firsat, profil);

        return new FirsatDto
        {
            Id = firsat.Id,
            Baslik = firsat.Baslik,
            Aciklama = firsat.Aciklama,
            FirsatTuru = firsat.FirsatTuru,
            Sehir = firsat.Sehir,
            SonBasvuruTarihi = firsat.SonBasvuruTarihi,
            SonKontrolTarihi = firsat.SonKontrolTarihi,
            GuncellikEtiketi = GuncellikDurumu.Hesapla(firsat.SonKontrolTarihi).Etiket,
            KaynakBaglantisi = firsat.KaynakBaglantisi,
            OnayDurumu = firsat.OnayDurumu,
            GerekliBolum = firsat.GerekliBolum,
            GerekliSinif = firsat.GerekliSinif,
            MinNotOrtalamasi = firsat.MinNotOrtalamasi,
            GerekliDilSeviyesi = firsat.GerekliDilSeviyesi,
            Uygunluk = new UygunlukDto
            {
                Durum = sonuc.Durum,
                Neden = sonuc.Neden,
                ProfilUyarilari = sonuc.ProfilUyarilari,
                IlanUyarilari = sonuc.IlanUyarilari
            }
        };
    }
}

public class UygunlukDto
{
    public string Durum { get; set; } = string.Empty;
    public string Neden { get; set; } = string.Empty;
    public List<string> ProfilUyarilari { get; set; } = new();
    public List<string> IlanUyarilari { get; set; } = new();
}

public class SayfaliFirsatCevabi
{
    public int ToplamKayit { get; set; }
    public int Sayfa { get; set; }
    public int SayfaBoyutu { get; set; }
    public int ToplamSayfa { get; set; }
    public List<FirsatDto> Kayitlar { get; set; } = new();
}
