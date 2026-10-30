using GencPusula.Web.Data;
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
            .Select(f => new FirsatDto
            {
                Id = f.Id,
                Baslik = f.Baslik,
                FirsatTuru = f.FirsatTuru,
                Sehir = f.Sehir,
                SonBasvuruTarihi = f.SonBasvuruTarihi,
                KaynakBaglantisi = f.KaynakBaglantisi
            })
            .ToListAsync();

        return Ok(firsatlar);
    }
}

public class FirsatDto
{
    public int Id { get; set; }
    public string Baslik { get; set; } = string.Empty;
    public string FirsatTuru { get; set; } = string.Empty;
    public string Sehir { get; set; } = string.Empty;
    public DateTime SonBasvuruTarihi { get; set; }
    public string KaynakBaglantisi { get; set; } = string.Empty;
}