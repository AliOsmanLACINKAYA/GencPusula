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

    public DbSet<Firsat> Firsatlar { get; set; }
    public DbSet<OgrenciProfili> OgrenciProfilleri { get; set; }
    public DbSet<BasvuruKaydi> BasvuruKayitlari { get; set; }
}