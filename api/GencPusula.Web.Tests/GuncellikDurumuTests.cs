using GencPusula.Web.Models;

namespace GencPusula.Web.Tests;

public class GuncellikDurumuTests
{
    private static readonly DateTime Bugun = new(2026, 11, 5);

    [Fact]
    public void Null_Tarih_Belirtilmedi_Doner()
    {
        var sonuc = GuncellikDurumu.Hesapla(null, Bugun);

        Assert.Equal("Belirtilmedi", sonuc.Etiket);
        Assert.Equal("bg-secondary", sonuc.Sinif);
    }

    [Theory]
    [InlineData(0, "Güncel", "bg-success")]
    [InlineData(30, "Güncel", "bg-success")]
    [InlineData(31, "Kontrol Edilmeli", "bg-warning text-dark")]
    [InlineData(90, "Kontrol Edilmeli", "bg-warning text-dark")]
    [InlineData(91, "Eski", "bg-danger")]
    public void Gun_Esikleri_Dogru_Etiket_Uretir(
        int gunOnce,
        string beklenenEtiket,
        string beklenenSinif)
    {
        var sonuc = GuncellikDurumu.Hesapla(Bugun.AddDays(-gunOnce), Bugun);

        Assert.Equal(beklenenEtiket, sonuc.Etiket);
        Assert.Equal(beklenenSinif, sonuc.Sinif);
    }
}
