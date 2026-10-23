using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GencPusula.Web.Pages;

public class FirsatlarModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Kategori { get; set; } = "Tumu";

    public void OnGet()
    {
        if (string.IsNullOrWhiteSpace(Kategori))
        {
            Kategori = "Tumu";
        }
    }
}