using System.Threading.Tasks;
using GencPusula.Web.Data;
using GencPusula.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GencPusula.Web.Services
{
    public class ProfilKayitServisi
    {
        private readonly ApplicationDbContext _context;

        public ProfilKayitServisi(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<KullaniciProfili?> KullaniciProfiliniGetirAsync(string kullaniciId)
        {
            if (string.IsNullOrWhiteSpace(kullaniciId))
            {
                return null;
            }

            return await _context.Set<KullaniciProfili>()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.KullaniciId == kullaniciId);
        }

        public async Task<KullaniciProfili> KaydetVeYenidenGetirAsync(KullaniciProfili profil, string kullaniciId)
        {
            profil.KullaniciId = kullaniciId;

            var mevcutProfil = await _context.Set<KullaniciProfili>()
                .FirstOrDefaultAsync(p => p.KullaniciId == kullaniciId);

            if (mevcutProfil == null)
            {
                _context.Set<KullaniciProfili>().Add(profil);
            }
            else
            {
                profil.Id = mevcutProfil.Id;
                _context.Entry(mevcutProfil).CurrentValues.SetValues(profil);
            }

            await _context.SaveChangesAsync();

            return await _context.Set<KullaniciProfili>()
                .AsNoTracking()
                .FirstAsync(p => p.KullaniciId == kullaniciId);
        }
    }
}