using System.ComponentModel.DataAnnotations;

namespace GencPusula.Web.Models;

public class Firsat
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Baslik { get; set; } = string.Empty;

    [Required]
    public string Aciklama { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string FirsatTuru { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Sehir { get; set; } = string.Empty;

    public DateTime SonTarih { get; set; }

    [Required]
    [Url]
    public string KaynakBaglantisi { get; set; } = string.Empty;

    public DateTime SonKontrolTarihi { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string GuncellikEtiketi { get; set; } = "Güncel";

    public bool OnaylandiMi { get; set; } = false;

    [MaxLength(150)]
    public string? ArananBolum { get; set; }

    public int? ArananSinif { get; set; }

    public decimal? MinimumNotOrtalamasi { get; set; }

    public ICollection<BasvuruKaydi> BasvuruKayitlari { get; set; } = new List<BasvuruKaydi>();
}