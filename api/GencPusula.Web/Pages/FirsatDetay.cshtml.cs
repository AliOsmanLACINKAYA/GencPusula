using System.Threading.Tasks;
using GencPusula.Web.Data;
using GencPusula.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GencPusula.Web.Pages
{
    public class FirsatDetayModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public FirsatDetayModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public Firsat? SeciliFirsat { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            SeciliFirsat = await _db.Firsatlar.FirstOrDefaultAsync(f => f.Id == id);

            if (SeciliFirsat == null)
            {
                return RedirectToPage("/Firsatlar");
            }

            return Page();
        }
    }
}