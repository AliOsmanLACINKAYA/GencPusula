namespace GencPusula.Web.Models;

public static class GuncellikDurumu
{
    public static (string Etiket, string Sinif) Hesapla(
        DateTime? sonKontrolTarihi,
        DateTime? bugun = null)
    {
        if (!sonKontrolTarihi.HasValue)
        {
            return ("Belirtilmedi", "bg-secondary");
        }

        var gun = ((bugun ?? DateTime.UtcNow).Date - sonKontrolTarihi.Value.Date).Days;

        return gun <= 30
            ? ("Güncel", "bg-success")
            : gun <= 90
                ? ("Kontrol Edilmeli", "bg-warning text-dark")
                : ("Eski", "bg-danger");
    }
}
