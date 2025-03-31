using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GitGodsLMS.Data;
using GitGodsLMS.Pages.Model;
using System.Text.Json;
using static System.Collections.Specialized.BitVector32;

namespace GitGodsLMS.Pages
{
    public class StudentClassRegistrationModel : PageModel
    {
        private readonly LMSPagesContext _context;

        public StudentClassRegistrationModel(LMSPagesContext context)
        {
            _context = context;
        }

        // List of all classes (for display on GET)
        public IList<Class> AllClasses { get; set; } = new List<Class>();
        // List of all of the current users classes
        // Used to update the session's data about a student's classes
        public List<Class> UserClasses { get; set; } = new List<Class>();

        // The set of class IDs the student is already registered for
        public HashSet<int> RegisteredClassIds { get; set; } = new HashSet<int>();

        // Getter and Setter for the total balance of the student.
        public decimal TotalBalance { get; set; }

        /// <summary>
        /// Handles GET: verify user is a student, load classes, and determine which are already registered.
        /// </summary>
        public IActionResult OnGet()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                // No user => redirect to login
                return RedirectToPage("/Index");
            }

            // Ensure user exists and is a student (not a professor)
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || user.isProfessor)
            {
                // If user is null or is a professor => back to login (or wherever)
                return RedirectToPage("/Index");
            }

            // Load all classes from the Classes table
            AllClasses = _context.Classes.ToList();

            // Build a set of class IDs the student is already registered for
            RegisteredClassIds = _context.StudentClasses
                .Where(sc => sc.UserId == user.Id)
                .Select(sc => sc.ClassId)
                .ToHashSet();

            TotalBalance = _context.StudentClasses
        .Where(sc => sc.UserId == user.Id)
        .Join(_context.Classes, sc => sc.ClassId, c => c.Id, (sc, c) => c.CreditHours)
        .Sum() * 100;


            return Page();
        }

        /// <summary>
        /// Called when student clicks "Register" button for a specific class.
        /// </summary>
        public IActionResult OnPostRegister(int classId)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Index");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || user.isProfessor)
            {
                return RedirectToPage("/Index");
            }

            // Check if class actually exists
            var theClass = _context.Classes.FirstOrDefault(c => c.Id == classId);
            if (theClass == null)
            {
                return RedirectToPage();
            }

            // Check if already registered
            bool alreadyRegistered = _context.StudentClasses
                .Any(sc => sc.UserId == user.Id && sc.ClassId == theClass.Id);

            if (!alreadyRegistered)
            {
                var registration = new StudentClass
                {
                    UserId = user.Id,
                    ClassId = theClass.Id
                };
                _context.StudentClasses.Add(registration);
                _context.SaveChanges();
            }

            // Update the session to add the class card after registration
            var userClassesJson = HttpContext.Session.GetString("UserClasses");

            if (!string.IsNullOrEmpty(userClassesJson))
            {
                AllClasses = JsonSerializer.Deserialize<List<Class>>(userClassesJson);
            }
            else
            {
                AllClasses = new List<Class>();
            }

            AllClasses.Add(theClass);
            HttpContext.Session.SetString("UserClasses", JsonSerializer.Serialize(AllClasses));

            // Reload the page (GET) to reflect new registration
            return RedirectToPage();
        }

        /// <summary>
        /// Called when student clicks "Drop Class" button for a specific class.
        /// </summary>
        public IActionResult OnPostDrop(int classId)
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Index");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || user.isProfessor)
            {
                return RedirectToPage("/Index");
            }

            // Look for the existing row in StudentClasses (link between this user & class)
            var registration = _context.StudentClasses
                .FirstOrDefault(sc => sc.UserId == user.Id && sc.ClassId == classId);

            if (registration != null)
            {
                // Remove => "drop" the class
                _context.StudentClasses.Remove(registration);
                _context.SaveChanges();
            }

            // Logic to remove a class card from the session after dropping class
            var userClassesJson = HttpContext.Session.GetString("UserClasses");

            if (!string.IsNullOrEmpty(userClassesJson))
            {
                UserClasses = JsonSerializer.Deserialize<List<Class>>(userClassesJson);

                var classToRemove = UserClasses.FirstOrDefault(c => c.Id == registration.ClassId);
                if (classToRemove != null)
                {
                    UserClasses.Remove(classToRemove);
                    HttpContext.Session.SetString("UserClasses", JsonSerializer.Serialize(UserClasses));
                }
            }

            // Reload the page (GET) to update the table
            return RedirectToPage();
        }
    }
}
