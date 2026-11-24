namespace GencPusula.Web.Models
{
    public class UygunlukGorunumu
    {
        public string Etiket { get; }
        public string RozetSinifi { get; }
        public string KartKenarlikSinifi { get; }
        public string KutuArkaPlanSinifi { get; }

        private UygunlukGorunumu(string etiket, string rozetSinifi, string kartKenarlikSinifi, string kutuArkaPlanSinifi)
        {
            Etiket = etiket;
            RozetSinifi = rozetSinifi;
            KartKenarlikSinifi = kartKenarlikSinifi;
            KutuArkaPlanSinifi = kutuArkaPlanSinifi;
        }

        public static UygunlukGorunumu Olustur(string? durum)
        {
            return durum switch
            {
                "Uygun" => new UygunlukGorunumu(
                    "Uygun",
                    "bg-success",
                    "border-success",
                    "alert-success"),
                "SartKarsilanmiyor" => new UygunlukGorunumu(
                    "Şart Karşılanmıyor",
                    "bg-danger",
                    "border-danger",
                    "alert-danger"),
                _ => new UygunlukGorunumu(
                    "Bilgi Eksik",
                    "bg-warning text-dark",
                    "border-warning",
                    "alert-warning")
            };
        }
    }
}