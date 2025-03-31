using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GitGodsLMS.Data; // Import the namespace for LMSPagesContext
using GitGodsLMS.Pages.Model; // Import the namespace for the User model
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GitGodsLMS.Pages.Shared
{
    public class SignUpModel : PageModel
    {
        private readonly LMSPagesContext _context; // Declare the _context field
        [AllowNull]
        public string ErrorMessage;

        public SignUpModel(LMSPagesContext context) // Inject LMSPagesContext
        {
            _context = context; // Assign the injected context to the private field
        }

        [BindProperty]

        public User user { get; set; }
        [BindProperty]
        public string confirmPass {  get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(user.FirstName) ||
                string.IsNullOrWhiteSpace(user.LastName) ||
                string.IsNullOrWhiteSpace(user.Birthdate) ||
                string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.Password) ||
                string.IsNullOrWhiteSpace(confirmPass)) 
            {
                ErrorMessage = "All fields are required.";
                return Page();
            }

            // Check if the user already exists
            var existingUser = _context.Users
                .FirstOrDefault(u => u.Email == user.Email);

            if (existingUser != null)
            {
                ErrorMessage = "That email is already in use.";
                return Page();
            }

            var currentPass = user.Password;
            if (currentPass.Length < 8)
            {
                ErrorMessage = "The password must be at least 8 characters long.";
                return Page();
            }
            var checkPass = confirmPass;
            if (currentPass != checkPass)
            {
                ErrorMessage = "The passwords do not match.";
                return Page();
            }

            // Add new user
            var newUser = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Birthdate = user.Birthdate,
                Email = user.Email,
                Password = user.Password,
                isProfessor = user.isProfessor

            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return RedirectToPage("/Index");
        }
    }
}
