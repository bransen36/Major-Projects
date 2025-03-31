using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GitGodsLMS.Data;
using GitGodsLMS.Pages.Model;
using System.Text.Json;

namespace GitGodsLMS.Pages.Class_Management
{
    public class DeleteModel : PageModel
    {
        private readonly GitGodsLMS.Data.LMSPagesContext _context;

        public DeleteModel(GitGodsLMS.Data.LMSPagesContext context)
        {
            _context = context;
        }

        public List<Class> UserClasses { get; set; } = new List<Class>();

        [BindProperty]
        public Class Class { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var section = await _context.Classes.FirstOrDefaultAsync(m => m.Id == id);

            if (section == null)
            {
                return NotFound();
            }
            else
            {
                Class = section;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var section = await _context.Classes.FindAsync(id);
            if (section != null)
            {
                Class = section;
                _context.Classes.Remove(Class);
                await _context.SaveChangesAsync();
            }

            var userClassesJson = HttpContext.Session.GetString("UserClasses");

            if (!string.IsNullOrEmpty(userClassesJson))
            {
                UserClasses = JsonSerializer.Deserialize<List<Class>>(userClassesJson);

                var classToRemove = UserClasses.FirstOrDefault(c => c.Id == section.Id);
                if (classToRemove != null)
                {
                    UserClasses.Remove(classToRemove);
                    HttpContext.Session.SetString("UserClasses", JsonSerializer.Serialize(UserClasses));
                }
            }

            return RedirectToPage("../Classes");
        }
    }
}
