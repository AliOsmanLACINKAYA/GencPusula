using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
// Doğrulama kurallarını bu dosayda kullanacağız.
namespace GencPusula.Web.Models
{
    public class Firsat
    {
        //Birincil anahtar ID olarak belirlendi
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Fırsat türü zorunludur.")]
        [StringLength(50)]
        [Display(Name = "Fırsat Türü")]
        public string FirsatTuru { get; set; } = string.Empty;

        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [StringLength(200)]
        [Display(Name = "Başlık")]
        public string Baslik { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        [Display(Name = "Açıklama")]
        public string Aciklama { get; set; } = string.Empty;

        [Required(ErrorMessage = "Son başvuru tarihi zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Son Başvuru Tarihi")]
        public DateTime SonBasvuruTarihi { get; set; }

        [Required(ErrorMessage = "Şehir bilgisi zorunludur.")]
        [StringLength(100)]
        [Display(Name = "Şehir Bilgisi")]
        public string Sehir { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kaynak bağlantısı zorunludur.")]
        [Url(ErrorMessage = "Geçerli bir web adresi (http:// veya https://) giriniz.")]
        [StringLength(500)]
        [Display(Name = "Kaynak Bağlantısı")]
        public string KaynakBaglantisi { get; set; } = string.Empty;

        public ICollection<BasvuruKaydi>? Basvurular { get; set; }
    }
}