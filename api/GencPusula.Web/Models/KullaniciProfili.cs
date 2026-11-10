using System;
using System.ComponentModel.DataAnnotations;

namespace GencPusula.Web.Models
{
    public class KullaniciProfili
    {
        public int Id { get; set; }

        [Required]
        public string KullaniciId { get; set; } = string.Empty;

        [Display(Name = "Ad Soyad")]
        [StringLength(100)]
        public string AdSoyad { get; set; } = string.Empty;


        // Üniversiteye gerek olmayabilir. Çünkü her üniversitenin ayrı ayrı içeriklerine ulaşmak bizi zorlayabilir.
        /*[Display(Name = "Üniversite")]
        [StringLength(150)]
        public string Universite { get; set; } = string.Empty;*/

        [Display(Name = "Bölüm")]
        [StringLength(150)]
        public string Bolum { get; set; } = string.Empty;

        [Display(Name = "Sınıf / Öğrenim Düzeyi")]
        [StringLength(50)]
        public string Sinif { get; set; } = string.Empty;

        [Display(Name = "İlgi Alanları")]
        [StringLength(500)]
        public string IlgiAlanlari { get; set; } = string.Empty;

        [Display(Name = "Tercih Edilen Fırsat Türleri")]
        [StringLength(200)]
        public string TercihEdilenTurler { get; set; } = string.Empty;

        [Display(Name = "Tercih Edilen Şehir")]
        [StringLength(100)]
        public string Sehir { get; set; } = string.Empty;

        [Display(Name = "Genel Not Ortalaması (AGNO)")]
        [Range(0.00, 4.00)]
        public decimal? GenelNotOrtalamasi { get; set; }

        [Display(Name = "Yabancı Dil Seviyesi")]
        [StringLength(50)]
        public string? YabanciDilSeviyesi { get; set; }

        [Display(Name = "Son Güncellenme Tarihi")]
        public DateTime GuncellenmeTarihi { get; set; } = DateTime.UtcNow;
    }
}