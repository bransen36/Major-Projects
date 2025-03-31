using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GitGodsLMS.Data;
using System.Linq;
using GitGodsLMS.Pages.Model;
using System.Text.Json;

namespace GitGodsLMS.Pages
{
    /// <summary>
    /// Model for the welcome page
    /// </summary>
    public class WelcomeModel : PageModel
    {
        private readonly LMSPagesContext _context;

        public WelcomeModel(LMSPagesContext context)
        {
            _context = context;
        }
        public bool IsProfessor { get; set; }
        public bool IsStudent { get; set; }
        public List<Class> UserClasses{get; set;}
        public List<AssignmentListItem> UpcomingAssignments { get; set; } = new();

        public void OnGet()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                Response.Redirect("/Index");
                return;
            }

            // Find the current user
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                Response.Redirect("/Index");
                return;
            }

            // Check if the user is a student (not a professor)
            IsProfessor = user.isProfessor;
            IsStudent = !user.isProfessor;

            if (IsStudent)
            {
                // 1) Get the user's classes
                var userClassIds = _context.StudentClasses
                    .Where(sc => sc.UserId == user.Id)
                    .Select(sc => sc.ClassId)
                    .ToList();

                // 2) Query for upcoming assignments:
                //    - where assignment's ClassId is in user's classes
                //    - exclude past-due assignments
                //    - order by earliest due date
                //    - take top 5
                var now = DateTime.Now;
                var upcomingAssignmentsQuery =
                    from assignment in _context.Assignments
                    join cls in _context.Classes on assignment.ClassId equals cls.Id
                    where userClassIds.Contains(assignment.ClassId)
                          && assignment.DueDateTime > now
                    orderby assignment.DueDateTime ascending
                    select new AssignmentListItem
                    {
                        AssignmentId = assignment.Id,
                        ClassName = cls.Name,
                        AssignmentTitle = assignment.Title,
                        DueDateTime = assignment.DueDateTime
                    };

                UpcomingAssignments = upcomingAssignmentsQuery.Take(5).ToList();
            }

             var userClassesJson = HttpContext.Session.GetString("UserClasses");

            if (!string.IsNullOrEmpty(userClassesJson))
            {
                UserClasses = JsonSerializer.Deserialize<List<Class>>(userClassesJson);
            }
            else
            {
                UserClasses = new List<Class>();
            }
        }

        public class AssignmentListItem
        {
            public int AssignmentId { get; set; }
            public string ClassName { get; set; } = string.Empty;
            public string AssignmentTitle { get; set; } = string.Empty;
            public DateTime DueDateTime { get; set; }
        }
    }
}
