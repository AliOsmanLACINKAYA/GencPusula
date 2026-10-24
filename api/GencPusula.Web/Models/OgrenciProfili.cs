using System.ComponentModel.DataAnnotations;

namespace GencPusula.Web.Models;

public class OgrenciProfili
{
    public int Id { get; set; }

    [Required]
    public string KullaniciId { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Bolum { get; set; } = string.Empty;

    [Range(1, 6)]
    public int Sinif { get; set; }

    [Range(0.0, 4.0)]
    public decimal? NotOrtalamasi { get; set; }

    [MaxLength(500)]
    public string? IlgiAlanlari { get; set; }

    public ICollection<BasvuruKaydi> BasvuruKayitlari { get; set; } = new List<BasvuruKaydi>();
}