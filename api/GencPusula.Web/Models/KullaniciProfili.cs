using System;
using System.ComponentModel.DataAnnotations;

namespace GencPusula.Web.Models
{
    public class KullaniciProfili
    {
        public int Id { get; set; }

        [Required]
        public string KullaniciId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad soyad zorunludur.")]
        [Display(Name = "Ad Soyad")]
        [StringLength(100, ErrorMessage = "Ad soyad en fazla 100 karakter olabilir.")]
        public string AdSoyad { get; set; } = string.Empty;


        // Üniversiteye gerek olmayabilir. Çünkü her üniversitenin ayrı ayrı içeriklerine ulaşmak bizi zorlayabilir.
        /*[Display(Name = "Üniversite")]
        [StringLength(150)]
        public string Universite { get; set; } = string.Empty;*/

        [Required(ErrorMessage = "Bölüm zorunludur.")]
        [Display(Name = "Bölüm")]
        [StringLength(150, ErrorMessage = "Bölüm en fazla 150 karakter olabilir.")]
        public string Bolum { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sınıf zorunludur.")]
        [Display(Name = "Sınıf / Öğrenim Düzeyi")]
        [StringLength(50, ErrorMessage = "Sınıf en fazla 50 karakter olabilir.")]
        public string Sinif { get; set; } = string.Empty;

        [Display(Name = "İlgi Alanları")]
        [StringLength(500, ErrorMessage = "İlgi alanları en fazla 500 karakter olabilir.")]
        public string? IlgiAlanlari { get; set; }

        [Display(Name = "Tercih Edilen Fırsat Türleri")]
        [StringLength(200, ErrorMessage = "Tercih edilen türler en fazla 200 karakter olabilir.")]
        public string? TercihEdilenTurler { get; set; }

        [Display(Name = "Tercih Edilen Şehir")]
        [StringLength(100, ErrorMessage = "Şehir en fazla 100 karakter olabilir.")]
        public string? Sehir { get; set; }

        [Display(Name = "Genel Not Ortalaması (AGNO)")]
        [Range(0.00, 4.00, ErrorMessage = "Genel not ortalaması 0 ile 4 arasında olmalıdır.")]
        public decimal? GenelNotOrtalamasi { get; set; }

        [Display(Name = "Yabancı Dil Seviyesi")]
        [StringLength(50, ErrorMessage = "Yabancı dil seviyesi en fazla 50 karakter olabilir.")]
        public string? YabanciDilSeviyesi { get; set; }

        [Display(Name = "Son Güncellenme Tarihi")]
        public DateTime GuncellenmeTarihi { get; set; } = DateTime.UtcNow;
    }
}
