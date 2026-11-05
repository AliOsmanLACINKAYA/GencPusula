using System.ComponentModel.DataAnnotations;
using GencPusula.Web.Models;

namespace GencPusula.Web.Tests;

public class FirsatValidationTests
{
    [Fact]
    public void Gecmis_SonBasvuruTarihi_Hata_Verir()
    {
        var firsat = new Firsat
        {
            SonBasvuruTarihi = DateTime.Today.AddDays(-1)
        };

        var sonuclar = firsat.Validate(new ValidationContext(firsat)).ToList();

        var hata = Assert.Single(sonuclar);
        Assert.Contains(nameof(Firsat.SonBasvuruTarihi), hata.MemberNames);
        Assert.Contains("geçmiş", hata.ErrorMessage);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Bugun_Veya_Gelecek_Tarih_Hata_Vermez(int gunSonra)
    {
        var firsat = new Firsat
        {
            SonBasvuruTarihi = DateTime.Today.AddDays(gunSonra)
        };

        Assert.Empty(firsat.Validate(new ValidationContext(firsat)));
    }
}
