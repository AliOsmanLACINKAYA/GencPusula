using GencPusula.Web.Models;

namespace GencPusula.Web.Services;

public sealed record UygunlukSonucu(
    string Durum,
    string Neden,
    List<string> ProfilUyarilari,
    List<string> IlanUyarilari);

public static class UygunlukKurallari
{
    public static UygunlukSonucu Degerlendir(Firsat firsat, KullaniciProfili? profil)
    {
        var bolum = profil?.Bolum?.Trim() ?? "";
        var sinif = profil?.Sinif?.Trim() ?? "";
        var not = profil?.GenelNotOrtalamasi;
        var dil = profil?.YabanciDilSeviyesi;

        var hatalar = new List<string>();
        var eksikAlanlar = new List<string>();
        var profilUyarilari = new List<string>();
        var ilanUyarilari = new List<string>();
        var uyan = 0;

        void Eksik(string ad)
        {
            eksikAlanlar.Add(ad);
            profilUyarilari.Add($"Profilinde {ad} bilgisi eksik.");
        }

        if (string.IsNullOrWhiteSpace(firsat.GerekliBolum))
            ilanUyarilari.Add("İlanda bölüm şartı belirtilmemiş.");
        else if (bolum.Length == 0)
            Eksik("bölüm");
        else if (!Parcala(firsat.GerekliBolum).Any(b => Sadele(bolum).Contains(Sadele(b))))
            hatalar.Add($"İlan {firsat.GerekliBolum} bölümlerine açık; profilin: {bolum}.");
        else
            uyan++;

        if (string.IsNullOrWhiteSpace(firsat.GerekliSinif))
            ilanUyarilari.Add("İlanda sınıf şartı belirtilmemiş.");
        else if (sinif.Length == 0)
            Eksik("sınıf");
        else if (!Parcala(firsat.GerekliSinif).Any(s => SinifAnahtar(s) == SinifAnahtar(sinif)))
            hatalar.Add($"İlan {firsat.GerekliSinif} sınıflarına açık; profilin: {sinif}.");
        else
            uyan++;

        if (!firsat.MinNotOrtalamasi.HasValue)
            ilanUyarilari.Add("İlanda not ortalaması şartı belirtilmemiş.");
        else if (not is not decimal ortalama)
            Eksik("not ortalaması");
        else if (ortalama < firsat.MinNotOrtalamasi.Value)
            hatalar.Add($"İlan en az {firsat.MinNotOrtalamasi:0.00} not ortalaması istiyor; profilin: {ortalama:0.00}.");
        else
            uyan++;

        var gerekliDil = DilSeviyesi(firsat.GerekliDilSeviyesi);
        var profilDil = DilSeviyesi(dil);
        if (gerekliDil == 0)
            ilanUyarilari.Add("İlanda yabancı dil şartı belirtilmemiş.");
        else if (profilDil == 0)
            Eksik("yabancı dil");
        else if (profilDil < gerekliDil)
            hatalar.Add($"İlan en az {firsat.GerekliDilSeviyesi} dil seviyesi istiyor; profilin: {dil}.");
        else
            uyan++;

        if (hatalar.Count > 0)
            return new("SartKarsilanmiyor", string.Join(" ", hatalar), profilUyarilari, ilanUyarilari);

        if (eksikAlanlar.Count > 0)
            return new("BilgiEksik",
                profil is null
                    ? "Karşılaştırma için profilini doldurman gerekiyor."
                    : $"Karşılaştırma için profilinde şu bilgiler eksik: {string.Join(", ", eksikAlanlar)}.",
                profilUyarilari, ilanUyarilari);

        if (uyan > 0)
            return new("Uygun", "Profilin ilandaki tüm şartları karşılıyor.", profilUyarilari, ilanUyarilari);

        return new("BilgiEksik",
            "İlanın uygunluk şartları belirtilmemiş; kesin sonuç için ilanı kontrol et.",
            profilUyarilari, ilanUyarilari);
    }

    private static string[] Parcala(string? metin) =>
        (metin ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static string Sadele(string? metin) => (metin ?? "")
        .Replace("İ", "i").Replace("I", "i").ToLowerInvariant()
        .Replace("ı", "i").Replace("ş", "s").Replace("ğ", "g")
        .Replace("ü", "u").Replace("ö", "o").Replace("ç", "c").Trim();

    private static string SinifAnahtar(string? metin) =>
    Sadele(metin)
        .Replace(" ", "")
        .Replace(".sinif", "")
        .Replace("sinif", "")
        .TrimEnd('.');

    private static int DilSeviyesi(string? metin)
    {
        var s = Sadele(metin);
        return s.Length >= 2 && s[0] >= 'a' && s[0] <= 'c' && s[1] >= '1' && s[1] <= '2'
            ? (s[0] - 'a' + 1) * 2 + (s[1] - '1')
            : 0;
    }
}
