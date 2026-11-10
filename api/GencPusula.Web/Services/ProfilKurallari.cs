using GencPusula.Web.Models;

namespace GencPusula.Web.Services;

public static class ProfilKurallari
{
    public static List<string> EksikAlanlar(KullaniciProfili? profil)
    {
        var eksikler = new List<string>();

        if (string.IsNullOrWhiteSpace(profil?.AdSoyad))
            eksikler.Add("Ad Soyad");

        if (string.IsNullOrWhiteSpace(profil?.Bolum))
            eksikler.Add("Bölüm");

        if (string.IsNullOrWhiteSpace(profil?.Sinif))
            eksikler.Add("Sınıf");

        return eksikler;
    }

    public static bool Tamamlandi(KullaniciProfili? profil) =>
        EksikAlanlar(profil).Count == 0;
}
