using System.ComponentModel.DataAnnotations;

namespace GencPusula.Web.Models;

public class BasvuruKaydi
{
    public int Id { get; set; }

    public int FirsatId { get; set; }

    public Firsat? Firsat { get; set; }

    public int OgrenciProfiliId { get; set; }

    public OgrenciProfili? OgrenciProfili { get; set; }

    [Required]
    [MaxLength(50)]
    public string BasvuruDurumu { get; set; } = "İlgileniyorum";

    [MaxLength(1000)]
    public string? KisiselNotlar { get; set; }

    public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;
}