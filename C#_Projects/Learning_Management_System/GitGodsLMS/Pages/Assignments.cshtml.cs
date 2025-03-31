using GitGodsLMS.Data;
using GitGodsLMS.Pages.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;

namespace GitGodsLMS.Pages
{
    public class AssignmentsModel : PageModel
    {
        private readonly LMSPagesContext _context;
        public AssignmentsModel(LMSPagesContext context)
        {
            _context = context;
        }

        // This will hold the new assignment from the form.
        [BindProperty]
        public Assignment Assignment { get; set; } = new Assignment();

        // List of assignments for display.
        public List<Assignment> Assignments { get; set; } = new List<Assignment>();

        // List of classes that the professor teaches.
        public List<Class> ProfessorClasses { get; set; } = new List<Class>();

        public bool IsProfessor { get; set; }

        public IActionResult OnGet()
        {
            // Check session for logged in user.
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Index");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return RedirectToPage("/Index");
            }

            // Determine if user is a professor.
            IsProfessor = user.isProfessor;

            if (!IsProfessor)
            {
                // Optionally redirect or show an error if a student somehow reaches this page.
                return RedirectToPage("/Index");
            }

            // Get the list of classes the professor is teaching.
            ProfessorClasses = _context.Classes.Where(c => c.ProfessorId == user.Id).ToList();

            // Get the assignments for the professor’s classes.
            var classIds = ProfessorClasses.Select(c => c.Id).ToList();
            Assignments = _context.Assignments.Where(a => classIds.Contains(a.ClassId)).ToList();

            return Page();
        }

        // Handle form submission to create a new assignment.
        public async Task<IActionResult> OnPostAsync()
        {
            // Check session and professor status (same as in OnGet).
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Index");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || !user.isProfessor)
            {
                return RedirectToPage("/Index");
            }

            if (!ModelState.IsValid)
            {
                // If validation fails, repopulate the professor classes list.
                ProfessorClasses = _context.Classes.Where(c => c.ProfessorId == user.Id).ToList();
                return Page();
            }

            // Save the new assignment.
            _context.Assignments.Add(Assignment);
            await _context.SaveChangesAsync();

            // Redirect to refresh the page and show the new assignment.
            return RedirectToPage();
        }

        // Handle deletion of an assignment.
        public async Task<IActionResult> OnPostDeleteAsync(int assignmentId)
        {
            var assignment = _context.Assignments.FirstOrDefault(a => a.Id == assignmentId);
            if (assignment != null)
            {
                _context.Assignments.Remove(assignment);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
