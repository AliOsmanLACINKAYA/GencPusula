using Microsoft.EntityFrameworkCore;

namespace GencPusula.Web.Data;

public static class KategoriListesi
{
    public static async Task<List<string>> YukleAsync(ApplicationDbContext db)
    {
        return await db.Kategoriler
            .AsNoTracking()
            .OrderBy(k => k.Ad)
            .Select(k => k.Ad)
            .ToListAsync();
    }
}
