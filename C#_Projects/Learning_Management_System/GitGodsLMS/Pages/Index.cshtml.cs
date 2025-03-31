using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GitGodsLMS.Data;
using GitGodsLMS.Pages.Model;
using System.Linq;
using System.Text.Json;

namespace GitGodsLMS.Pages
{
    /// <summary>
    /// Model for the Login page labeled as index
    /// Handles user authentication and Login logic.
    /// </summary>
    public class IndexModel : PageModel
    {
        /// <summary>
        /// Logger instance for logging information, warnings, and errors.
        /// </summary>
        private readonly ILogger<IndexModel> _logger;

        /// <summary>
        /// Database context for accessing LMS pages data.
        /// </summary>
        private readonly LMSPagesContext _context;

        /// <summary>
        /// Initializes a new instance of the IndexModel class
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="context"></param>
        public IndexModel(ILogger<IndexModel> logger, LMSPagesContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Gets or sets the user's email address entered on the 
        /// login form.
        /// </summary>
        [BindProperty]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's password entered on the login form
        /// </summary>
        [BindProperty]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the error message to display on the login page
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        public bool IsProfessor { get; set; }
        public bool IsStudent { get; set; }

        public void OnGet()
        {
        }

        /// <summary>
        /// Handles post requests when the login form is submitted
        /// Authenticates the user and redirects to the welcome page
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPost()
        {
            // Retrieve the user from the database based on email and password
            var user = _context.Users
            .FirstOrDefault(u => u.Email == Email && u.Password == Password);

            // Case of no user
            if (user == null)
            {
                ErrorMessage = "Invalid Email or Password.";
                return Page();
            }

            IsProfessor = user.isProfessor;
            IsStudent = !user.isProfessor;

            List<Class> userClasses;

            if (IsProfessor)
            {
                userClasses = _context.Classes
                       .Where(c => c.ProfessorId == user.Id)
                       .ToList();
                HttpContext.Session.SetString("UserRole", "Professor");
            }
            else
            {
                userClasses = _context.StudentClasses
                    .Where(sc => sc.UserId == user.Id)
                    .Select(sc => sc.Class!)
                    .ToList();
                HttpContext.Session.SetString("UserRole", "Student");
            }

            //Store the user email in the session for authentication
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("UserClasses", JsonSerializer.Serialize(userClasses));


            return RedirectToPage("/Welcome");
        }
    }
}
