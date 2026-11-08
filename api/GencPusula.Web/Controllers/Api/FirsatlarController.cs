using GencPusula.Web.Data;
using GencPusula.Web.Models;
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

    [HttpGet]
    public async Task<IActionResult> Listele()
    {
        var firsatlar = await _db.Firsatlar
            .AsNoTracking()
            .OrderBy(f => f.SonBasvuruTarihi)
            .ToListAsync();

        return Ok(firsatlar.Select(FirsatDto.Olustur).ToList());
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

        return Ok(new SayfaliFirsatCevabi
        {
            ToplamKayit = toplamKayit,
            Sayfa = sayfa,
            SayfaBoyutu = sayfaBoyutu,
            ToplamSayfa = (int)Math.Ceiling((double)toplamKayit / sayfaBoyutu),
            Kayitlar = kayitlar.Select(FirsatDto.Olustur).ToList()
        });
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

        return Ok(FirsatDto.Olustur(firsat));
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

    public static FirsatDto Olustur(Firsat firsat)
    {
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
            KaynakBaglantisi = firsat.KaynakBaglantisi
        };
    }
}

public class SayfaliFirsatCevabi
{
    public int ToplamKayit { get; set; }
    public int Sayfa { get; set; }
    public int SayfaBoyutu { get; set; }
    public int ToplamSayfa { get; set; }
    public List<FirsatDto> Kayitlar { get; set; } = new();
}
