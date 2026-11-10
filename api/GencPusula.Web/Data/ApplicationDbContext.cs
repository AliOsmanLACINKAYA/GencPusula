using GencPusula.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GencPusula.Web.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    [DbFunction("translate", "pg_catalog")]
    public static string Translate(string metin, string kaynak, string hedef) =>
        throw new NotSupportedException();

    public DbSet<Firsat> Firsatlar { get; set; }
    public DbSet<Kategori> Kategoriler { get; set; }
    public DbSet<KullaniciProfili> KullaniciProfilleri { get; set; }
    public DbSet<BasvuruKaydi> BasvuruKayitlari { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Firsat>()
            .HasOne(f => f.Kategori)
            .WithMany(k => k.Firsatlar)
            .HasForeignKey(f => f.FirsatTuru)
            .HasPrincipalKey(k => k.Ad)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<KullaniciProfili>()
            .HasIndex(p => p.KullaniciId)
            .IsUnique();

        builder.Entity<KullaniciProfili>()
            .Property(p => p.IlgiAlanlari)
            .IsRequired();

        builder.Entity<KullaniciProfili>()
            .Property(p => p.TercihEdilenTurler)
            .IsRequired();

        builder.Entity<KullaniciProfili>()
            .Property(p => p.Sehir)
            .IsRequired();

        builder.Entity<BasvuruKaydi>()
            .HasIndex(b => new { b.KullaniciProfiliId, b.FirsatId })
            .IsUnique();
    }
}