using GencPusula.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GencPusula.Web.Controllers.Api;

[ApiController]
[Route("api/kategoriler")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class KategorilerController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public KategorilerController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Listele()
    {
        var kategoriler = await KategoriListesi.YukleAsync(_db);
        return Ok(kategoriler);
    }
}
