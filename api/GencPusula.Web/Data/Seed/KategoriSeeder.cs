using GencPusula.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GencPusula.Web.Data.Seed;

public static class KategoriSeeder
{
    public static async Task CalistirAsync(ApplicationDbContext db)
    {
        var kategoriAdlari = new[] { "Burs", "Staj", "Etkinlik", "Yarisma" };
        var mevcutKategoriler = await db.Kategoriler
            .Select(k => k.Ad)
            .ToListAsync();

        var eksikKategoriler = kategoriAdlari
            .Except(mevcutKategoriler)
            .Select(ad => new Kategori { Ad = ad })
            .ToList();

        if (eksikKategoriler.Count > 0)
        {
            db.Kategoriler.AddRange(eksikKategoriler);
            await db.SaveChangesAsync();
        }
    }
}
