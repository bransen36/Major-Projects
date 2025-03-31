using System.Text.Json;
using GitGodsLMS.Data;
using GitGodsLMS.Pages.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static GitGodsLMS.Pages.WelcomeModel;

namespace GitGodsLMS.Pages
{
    public class ClassesModel : PageModel
    {
        private readonly LMSPagesContext _context;
        public ClassesModel(LMSPagesContext context)
        {
            _context = context;
        }

        public List<Class> UserClasses { get; set; } = new List<Class>();
        public List<AssignmentListItem> UpcomingAssignments { get; set; } = new();

        public bool IsProfessor { get; set; }
        public bool IsStudent { get; set; }

        [BindProperty]
        public Assignment Assignment { get; set; } = new Assignment();

        public List<Assignment> Assignments {get; set;} =  new();

        public List<AssignmentSubmission> AssignmentSubmissions {get; set;} = new();

        public IActionResult OnGet()
        {
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

            IsProfessor = user.isProfessor;
            IsStudent = !user.isProfessor;

            //if (IsProfessor)
            //{
            //    UserClasses = _context.Classes
            //        .Where(c => c.ProfessorId == user.Id)
            //        .ToList();
            //}
            //else
            //{
            //    UserClasses = _context.StudentClasses
            //        .Where(sc => sc.UserId == user.Id)
            //        .Select(sc => sc.Class!)
            //        .ToList();
            //}


            var userClassesJson = HttpContext.Session.GetString("UserClasses");

            if (!string.IsNullOrEmpty(userClassesJson))
            {
                UserClasses = JsonSerializer.Deserialize<List<Class>>(userClassesJson);
            }
            else
            {
                UserClasses = new List<Class>();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCreateAssignmentAsync()
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
                UserClasses = _context.Classes.Where(c => c.ProfessorId == user.Id).ToList();
                return Page();
            }

            Assignment.CreationDateTime = DateTime.UtcNow;
            _context.Assignments.Add(Assignment);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public double GetGrade(int ClassID){
            double classGrade;
            int totMaxPoints=0;
            int totGrade=0;
            var sClass = _context.Classes.FirstOrDefault(c => c.Id == ClassID);
            Assignments= _context.Assignments.Where(a => a.ClassId == sClass.Id).ToList();
            foreach(Assignment a in Assignments){
                AssignmentSubmissions = _context.AssignmentSubmissions.Where(aS=>aS.AssignmentId == a.Id).ToList();
                int assignmentGrade = 0;
                foreach(AssignmentSubmission aS in AssignmentSubmissions){
                    if(aS.PointsAwarded != null && aS.PointsAwarded > assignmentGrade){
                        assignmentGrade= (int)aS.PointsAwarded;
                        }
                    }
                    totGrade += assignmentGrade;
                    totMaxPoints += a.MaxPoints;
            }
            classGrade = ((double)totGrade/(double)totMaxPoints)*100.0;

            return classGrade;
        }
    }
}
