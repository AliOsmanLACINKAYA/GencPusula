using System.ComponentModel.DataAnnotations;

namespace GencPusula.Web.Models;

public class Kategori
{
    [Key]
    [StringLength(50)]
    public string Ad { get; set; } = string.Empty;

    public ICollection<Firsat> Firsatlar { get; set; } = new List<Firsat>();
}
