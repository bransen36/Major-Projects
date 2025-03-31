using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GitGodsLMS.Data;
using GitGodsLMS.Pages.Model;
using System.Text.Json;

namespace GitGodsLMS.Pages.Class_Management
{
    public class ProfessorClassCreationModel : PageModel
    {
        private readonly LMSPagesContext _context;

        public ProfessorClassCreationModel(LMSPagesContext context)
        {
            _context = context;
        }

        public List<Class> UserClasses { get; set; } = new List<Class>();

        [BindProperty]
        public Class NewClass { get; set; } = new Class();

        public IActionResult OnGet()
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


            return Page();
        }

        public IActionResult OnPost()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Index");

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || !user.isProfessor)
            {
                return RedirectToPage("/Index");
            }

            NewClass.ProfessorId = user.Id;

            var meetingDays = new List<string>();

            if (NewClass.Sunday) meetingDays.Add("Sunday");
            if (NewClass.Monday) meetingDays.Add("Monday");
            if (NewClass.Tuesday) meetingDays.Add("Tuesday");
            if (NewClass.Wednesday) meetingDays.Add("Wednesday");
            if (NewClass.Thursday) meetingDays.Add("Thursday");
            if (NewClass.Friday) meetingDays.Add("Friday");
            if (NewClass.Saturday) meetingDays.Add("Saturday");

            // Join the selected days with commas
            NewClass.MeetingTimes = string.Join(", ", meetingDays);

            // Append start and end times
            NewClass.MeetingTimes += $" {NewClass.StartTime} - {NewClass.EndTime}";


            _context.Classes.Add(NewClass);
            _context.SaveChanges();

            var userClassesJson = HttpContext.Session.GetString("UserClasses");

            if (!string.IsNullOrEmpty(userClassesJson))
            {
                UserClasses = JsonSerializer.Deserialize<List<Class>>(userClassesJson);
            }
            else
            {
                UserClasses = new List<Class>();
            }

            UserClasses.Add(NewClass);
            HttpContext.Session.SetString("UserClasses", JsonSerializer.Serialize(UserClasses));
            return RedirectToPage("/Classes");
        }
    }
}
