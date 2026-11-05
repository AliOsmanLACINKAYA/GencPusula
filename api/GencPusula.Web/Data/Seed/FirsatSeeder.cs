using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GencPusula.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GencPusula.Web.Data.Seed
{
    public static class FirsatSeeder
    {
        public static async Task CalistirAsync(ApplicationDbContext db)
        {
            if (await db.Firsatlar.AnyAsync())
            {
                return;
            }

            var firsatlar = new List<Firsat>
            {
                new Firsat
                {
                    FirsatTuru = "Burs",
                    Baslik = "Anadolu Teknoloji Bursu",
                    Aciklama = "Mühendislik ve teknoloji alanında öğrenim gören öğrencilere yönelik örnek burs programı.",
                    SonBasvuruTarihi = DateTime.UtcNow.AddDays(15),
                    Sehir = "Ankara",
                    KaynakBaglantisi = "https://example.com/burs/anadolu-teknoloji",
                    SonKontrolTarihi = DateTime.UtcNow.Date
                },
                new Firsat
                {
                    FirsatTuru = "Staj",
                    Baslik = "Ege Yazılım Staj Programı",
                    Aciklama = "Yazılım geliştirme alanında kendini geliştirmek isteyen öğrenciler için örnek staj programı.",
                    SonBasvuruTarihi = DateTime.UtcNow.AddDays(25),
                    Sehir = "İzmir",
                    KaynakBaglantisi = "https://example.com/staj/ege-yazilim",
                    SonKontrolTarihi = DateTime.UtcNow.Date
                },
                new Firsat
                {
                    FirsatTuru = "Yarisma",
                    Baslik = "Ulusal Mobil Uygulama Yarışması",
                    Aciklama = "Üniversite öğrencilerinin mobil uygulama projeleriyle katılabileceği örnek yarışma.",
                    SonBasvuruTarihi = DateTime.UtcNow.AddDays(35),
                    Sehir = "İstanbul",
                    KaynakBaglantisi = "https://example.com/yarisma/mobil-uygulama",
                    SonKontrolTarihi = DateTime.UtcNow.Date
                },
                new Firsat
                {
                    FirsatTuru = "Etkinlik",
                    Baslik = "Kariyer Günleri 2026",
                    Aciklama = "Sektör temsilcileriyle tanışma ve kariyer planlama etkinliği.",
                    SonBasvuruTarihi = DateTime.UtcNow.AddDays(45),
                    Sehir = "Bursa",
                    KaynakBaglantisi = "https://example.com/etkinlik/kariyer-gunleri",
                    SonKontrolTarihi = DateTime.UtcNow.Date
                },
                new Firsat
                {
                    FirsatTuru = "Burs",
                    Baslik = "Kuzey Kampüs Başarı Bursu",
                    Aciklama = "Akademik başarıyı desteklemek için hazırlanmış örnek burs programı.",
                    SonBasvuruTarihi = DateTime.UtcNow.AddDays(55),
                    Sehir = "Trabzon",
                    KaynakBaglantisi = "https://example.com/burs/kuzey-kampus",
                    SonKontrolTarihi = DateTime.UtcNow.Date
                },
                new Firsat
                {
                    FirsatTuru = "Staj",
                    Baslik = "Dijital Pazarlama Stajı",
                    Aciklama = "Sosyal medya ve dijital pazarlama alanında örnek staj ilanı.",
                    SonBasvuruTarihi = DateTime.UtcNow.AddDays(65),
                    Sehir = "İstanbul",
                    KaynakBaglantisi = "https://example.com/staj/dijital-pazarlama",
                    SonKontrolTarihi = DateTime.UtcNow.Date
                },
                new Firsat
                {
                    FirsatTuru = "Burs",
                    Baslik = "TÜBİTAK Lisans Bursu",
                    Aciklama = "Lisans öğrencilerinin akademik ve araştırma çalışmalarını destekleyen örnek burs programı.",
                    SonBasvuruTarihi = DateTime.UtcNow.AddDays(20),
                    Sehir = "Ankara",
                    KaynakBaglantisi = "https://example.com/burs/tubitak-lisans",
                    SonKontrolTarihi = DateTime.UtcNow.Date
                },
                new Firsat
                {
                    FirsatTuru = "Burs",
                    Baslik = "Teknoloji Vakfı Başarı Bursu",
                    Aciklama = "Teknoloji alanında öğrenim gören başarılı öğrencilere yönelik örnek burs.",
                    SonBasvuruTarihi = DateTime.UtcNow.AddDays(40),
                    Sehir = "İstanbul",
                    KaynakBaglantisi = "https://example.com/burs/teknoloji-vakfi",
                    SonKontrolTarihi = DateTime.UtcNow.Date
                },
                new Firsat
                {
                    FirsatTuru = "Staj",
                    Baslik = "Savunma Sanayii Staj Programı",
                    Aciklama = "Mühendislik öğrencileri için savunma sanayii alanında örnek staj programı.",
                    SonBasvuruTarihi = DateTime.UtcNow.AddDays(30),
                    Sehir = "Ankara",
                    KaynakBaglantisi = "https://example.com/staj/savunma-sanayii",
                    SonKontrolTarihi = DateTime.UtcNow.Date
                },
                new Firsat
                {
                    FirsatTuru = "Staj",
                    Baslik = "Veri Bilimi Uzun Dönem Stajı",
                    Aciklama = "Veri analizi ve makine öğrenmesi alanında uzun dönemli örnek staj ilanı.",
                    SonBasvuruTarihi = DateTime.UtcNow.AddDays(50),
                    Sehir = "İstanbul",
                    KaynakBaglantisi = "https://example.com/staj/veri-bilimi",
                    SonKontrolTarihi = DateTime.UtcNow.Date
                },
                new Firsat
                {
                    FirsatTuru = "Yarisma",
                    Baslik = "Akıllı Şehirler Hackathonu",
                    Aciklama = "Şehir problemlerine teknolojiyle çözüm geliştiren öğrenciler için örnek hackathon.",
                    SonBasvuruTarihi = DateTime.UtcNow.AddDays(18),
                    Sehir = "Eskişehir",
                    KaynakBaglantisi = "https://example.com/yarisma/akilli-sehirler",
                    SonKontrolTarihi = DateTime.UtcNow.Date
                },
                new Firsat
                {
                    FirsatTuru = "Yarisma",
                    Baslik = "Üniversiteler Arası Girişimcilik Yarışması",
                    Aciklama = "Öğrenci ekiplerinin girişimcilik fikirlerini sunduğu örnek yarışma.",
                    SonBasvuruTarihi = DateTime.UtcNow.AddDays(60),
                    Sehir = "İzmir",
                    KaynakBaglantisi = "https://example.com/yarisma/girisimcilik",
                    SonKontrolTarihi = DateTime.UtcNow.Date
                },
                new Firsat
                {
                    FirsatTuru = "Etkinlik",
                    Baslik = "Kariyer Zirvesi 2026",
                    Aciklama = "Sektör liderleriyle tanışma ve kariyer planlama odaklı örnek etkinlik.",
                    SonBasvuruTarihi = DateTime.UtcNow.AddDays(12),
                    Sehir = "Bursa",
                    KaynakBaglantisi = "https://example.com/etkinlik/kariyer-zirvesi",
                    SonKontrolTarihi = DateTime.UtcNow.Date
                },
                new Firsat
                {
                    FirsatTuru = "Etkinlik",
                    Baslik = "Yapay Zeka Zirvesi",
                    Aciklama = "Yapay zeka alanındaki güncel gelişmelerin ele alındığı örnek öğrenci etkinliği.",
                    SonBasvuruTarihi = DateTime.UtcNow.AddDays(35),
                    Sehir = "Ankara",
                    KaynakBaglantisi = "https://example.com/etkinlik/yapay-zeka",
                    SonKontrolTarihi = DateTime.UtcNow.Date
                },
                new Firsat
                {
                    FirsatTuru = "Burs",
                    Baslik = "Dijital Dönüşüm Bursu",
                    Aciklama = "Dijital becerilerini geliştirmek isteyen öğrencilere yönelik örnek burs programı.",
                    SonBasvuruTarihi = DateTime.UtcNow.AddDays(70),
                    Sehir = "Konya",
                    KaynakBaglantisi = "https://example.com/burs/dijital-donusum",
                    SonKontrolTarihi = DateTime.UtcNow.Date
                },
                new Firsat
                {
                    FirsatTuru = "Staj",
                    Baslik = "Uzaktan Staj Fırsatı",
                    Aciklama = "Şehir bağımsız olarak uzaktan çalışılabilecek örnek staj ilanı.",
                    SonBasvuruTarihi = DateTime.UtcNow.AddDays(25),
                    Sehir = "Online",
                    KaynakBaglantisi = "https://example.com/staj/uzaktan-staj",
                    SonKontrolTarihi = DateTime.UtcNow.Date
                }
            };

            await db.Firsatlar.AddRangeAsync(firsatlar);
            await db.SaveChangesAsync();
        }
    }
}