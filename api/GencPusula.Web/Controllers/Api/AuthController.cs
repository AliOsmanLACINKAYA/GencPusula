using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GencPusula.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GencPusula.Web.Controllers.Api;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<IdentityUser> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterIstegi istek)
    {
        var email = istek.Email.Trim();

        var mevcutKullanici = await _userManager.FindByEmailAsync(email);
        if (mevcutKullanici is not null)
        {
            return BadRequest(new
            {
                kod = "EpostaKullanimda",
                mesaj = "Bu e-posta adresi zaten kayıtlı."
            });
        }

        var kullanici = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var olusturmaSonucu = await _userManager.CreateAsync(kullanici, istek.Parola);
        if (!olusturmaSonucu.Succeeded)
        {
            return BadRequest(new
            {
                kod = "KayitBasarisiz",
                mesaj = string.Join(" ",
                    olusturmaSonucu.Errors.Select(e => e.Description))
            });
        }

        var rolSonucu = await _userManager.AddToRoleAsync(kullanici, Roller.Kullanici);
        if (!rolSonucu.Succeeded)
        {
            await _userManager.DeleteAsync(kullanici);

            return BadRequest(new
            {
                kod = "RolAtanamadi",
                mesaj = "Kullanıcı rolü atanamadı."
            });
        }

        return StatusCode(201, await TokenUretAsync(kullanici));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginIstegi istek)
    {
        var kullanici = await _userManager.FindByEmailAsync(istek.Email.Trim());

        if (kullanici is null ||
            !await _userManager.CheckPasswordAsync(kullanici, istek.Parola))
        {
            return Unauthorized(new
            {
                kod = "GecersizKimlik",
                mesaj = "E-posta veya parola hatalı."
            });
        }

        return Ok(await TokenUretAsync(kullanici));
    }

    private async Task<AuthCevabi> TokenUretAsync(IdentityUser kullanici)
    {
        var roller = await _userManager.GetRolesAsync(kullanici);

        // Hafta 1'de rol atanmadan açılmış eski hesaplar için.
        if (roller.Count == 0)
        {
            await _userManager.AddToRoleAsync(kullanici, Roller.Kullanici);
            roller = await _userManager.GetRolesAsync(kullanici);
        }

        var anahtar = _configuration["Jwt:Key"]!;
        var issuer = _configuration["Jwt:Issuer"] ?? "GencPusula";
        var audience = _configuration["Jwt:Audience"] ?? "GencPusulaMobile";
        var bitisTarihi = DateTime.UtcNow.AddDays(7);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, kullanici.Id),
            new(JwtRegisteredClaimNames.Email, kullanici.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var rol in roller)
            claims.Add(new Claim("role", rol));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: bitisTarihi,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(anahtar)),
                SecurityAlgorithms.HmacSha256));

        return new AuthCevabi
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            GecerlilikBitis = bitisTarihi,
            KullaniciId = kullanici.Id,
            Email = kullanici.Email ?? string.Empty,
            Roller = roller.ToList()
        };
    }
}

public class RegisterIstegi
{
    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parola zorunludur.")]
    [MinLength(6, ErrorMessage = "Parola en az 6 karakter olmalıdır.")]
    public string Parola { get; set; } = string.Empty;
}

public class LoginIstegi
{
    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parola zorunludur.")]
    public string Parola { get; set; } = string.Empty;
}

public class AuthCevabi
{
    public string Token { get; set; } = string.Empty;
    public DateTime GecerlilikBitis { get; set; }
    public string KullaniciId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roller { get; set; } = new();
}