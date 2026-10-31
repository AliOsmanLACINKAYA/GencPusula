using GencPusula.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GencPusula.Web.Data.Seed;

public static class FirsatSeeder
{
    public static async Task CalistirAsync(ApplicationDbContext db)
    {
        if (await db.Firsatlar.AnyAsync())
            return;

        db.Firsatlar.AddRange(
            new Firsat
            {
                FirsatTuru = "Burs",
                Baslik = "Anadolu Teknoloji Bursu",
                Aciklama = "Mühendislik ve teknoloji alanında öğrenim gören öğrencilere yönelik örnek burs programı.",
                SonBasvuruTarihi = DateTime.UtcNow.AddDays(15),
                Sehir = "Ankara",
                KaynakBaglantisi = "https://example.com/burs/anadolu-teknoloji"
            },
            new Firsat
            {
                FirsatTuru = "Staj",
                Baslik = "Ege Yazılım Staj Programı",
                Aciklama = "Yazılım geliştirme alanında kendini geliştirmek isteyen öğrenciler için örnek staj programı.",
                SonBasvuruTarihi = DateTime.UtcNow.AddDays(25),
                Sehir = "İzmir",
                KaynakBaglantisi = "https://example.com/staj/ege-yazilim"
            },
            new Firsat
            {
                FirsatTuru = "Yarisma",
                Baslik = "Ulusal Mobil Uygulama Yarışması",
                Aciklama = "Üniversite öğrencilerinin mobil uygulama projeleriyle katılabileceği örnek yarışma.",
                SonBasvuruTarihi = DateTime.UtcNow.AddDays(35),
                Sehir = "İstanbul",
                KaynakBaglantisi = "https://example.com/yarisma/mobil-uygulama"
            },
            new Firsat
            {
                FirsatTuru = "Etkinlik",
                Baslik = "Kariyer Günleri 2026",
                Aciklama = "Sektör temsilcileriyle tanışma ve kariyer planlama etkinliği.",
                SonBasvuruTarihi = DateTime.UtcNow.AddDays(45),
                Sehir = "Bursa",
                KaynakBaglantisi = "https://example.com/etkinlik/kariyer-gunleri"
            },
            new Firsat
            {
                FirsatTuru = "Burs",
                Baslik = "Kuzey Kampüs Başarı Bursu",
                Aciklama = "Akademik başarıyı desteklemek için hazırlanmış örnek burs programı.",
                SonBasvuruTarihi = DateTime.UtcNow.AddDays(55),
                Sehir = "Trabzon",
                KaynakBaglantisi = "https://example.com/burs/kuzey-kampus"
            },
            new Firsat
            {
                FirsatTuru = "Staj",
                Baslik = "Dijital Pazarlama Stajı",
                Aciklama = "Sosyal medya ve dijital pazarlama alanında örnek staj ilanı.",
                SonBasvuruTarihi = DateTime.UtcNow.AddDays(65),
                Sehir = "İstanbul",
                KaynakBaglantisi = "https://example.com/staj/dijital-pazarlama"
            });

        await db.SaveChangesAsync();
    }
}