using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GitGodsLMS.Data;
using GitGodsLMS.Pages.Model;
using static GitGodsLMS.Pages.WelcomeModel;

namespace GitGodsLMS.Pages.Class_Management
{
    public class ClassDetailsModel : PageModel
    {
        private readonly LMSPagesContext _context;

        public ClassDetailsModel(LMSPagesContext context)
        {
            _context = context;
        }

        public Class MyClass { get; set; }
        public bool IsProfessor { get; set; }
        public bool IsStudent { get; set; }
        [BindProperty]
        public Assignment Assignment { get; set; } = new Assignment();
        public List<Assignment> ClassAssignments { get; set; } = new();

        public void OnGet(int classId)
        {
            MyClass = _context.Classes.FirstOrDefault(c => c.Id == classId);

            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                RedirectToPage("/Index");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                RedirectToPage("/Index");
            }

            IsProfessor = user.isProfessor;
            IsStudent = !user.isProfessor;

            if(IsStudent)
            {
                var classAssignments = _context.Assignments.Where(a => a.ClassId == classId);

                ClassAssignments = classAssignments.ToList();
            }

            if (IsProfessor)
            {
                var professorAssignments = _context.Assignments.Where(a => a.Class.ProfessorId == HttpContext.Session.GetInt32("UserId")
                    && a.ClassId == classId);

                ClassAssignments = professorAssignments.ToList();
            }

        }

        public async Task<IActionResult> OnPostCreateAssignmentAsync(int classId)
        {
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
                return Page();
            }

            Assignment.ClassId = classId;
            _context.Assignments.Add(Assignment);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

    }
}
