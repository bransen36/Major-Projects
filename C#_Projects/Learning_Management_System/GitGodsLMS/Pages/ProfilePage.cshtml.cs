using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GitGodsLMS.Data; // Namespace for the database context
using GitGodsLMS.Pages.Model; // Namespace for the User model
using System.Linq;

namespace GitGodsLMS.Pages
{
    public class ProfilePageModel : PageModel
    {
        private readonly LMSPagesContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProfilePageModel(LMSPagesContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public string Address1 { get; set; } = string.Empty;

        [BindProperty]
        public string Address2 { get; set; } = string.Empty;

        [BindProperty]
        public string City { get; set; } = string.Empty;

        [BindProperty]
        public string State { get; set; } = string.Empty;

        public User? User { get; private set; }

        [BindProperty]
        public IFormFile ProfilePicture { get; set; }

        public string ProfilePicturePath { get; set; }

        public void OnGet()
        {
            string? currentUserEmail = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(currentUserEmail))
            {
                // redirect if not logged in
                Response.Redirect("/Index");
                return;
            }

            User = _context.Users.FirstOrDefault(u => u.Email == currentUserEmail);

            if (User != null)
            {
                // set your address fields...
                Address1 = User.Address1 ?? string.Empty;
                Address2 = User.Address2 ?? string.Empty;
                City = User.City ?? string.Empty;
                State = User.State ?? string.Empty;

                // If the user *has* a ProfilePicturePath, use it; else default:
                ProfilePicturePath = User.ProfilePicturePath ?? "/uploads/default-profile.png";
            }
            else
            {
                // If the user could not be found in the database, set a default path
                ProfilePicturePath = "/uploads/default-profile.png";
            }
        }

        public IActionResult OnPostUploadPicture()
        {
            try
            {
                // Fetch the current logged-in user's email
                string? currentUserEmail = HttpContext.Session.GetString("Email");
                if (string.IsNullOrEmpty(currentUserEmail))
                {
                    // Handle not logged in
                    Response.Redirect("/Index");
                    return Page();
                }

                var user = _context.Users.FirstOrDefault(u => u.Email == currentUserEmail);
                if (user == null)
                {
                    return RedirectToPage("/Error");
                }

                if (ProfilePicture != null)
                {
                    // Ensure uploads folder exists
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    // Create unique filename, e.g. "userId_filename"
                    var fileName = $"{user.Id}_{Path.GetFileName(ProfilePicture.FileName)}";
                    var uploadPath = Path.Combine(uploadsFolder, fileName);

                    // Save to disk
                    using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                    {
                        ProfilePicture.CopyTo(fileStream);
                    }

                    // Build relative path for <img src="...">
                    var relativePath = $"/uploads/{fileName}";

                    // Save path in DB
                    user.ProfilePicturePath = relativePath;
                    _context.SaveChanges();
                }

                // After uploading, redirect so OnGet() reloads the new picture
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                // Log or handle error
                System.Diagnostics.Debug.WriteLine($"Error in OnPostUploadPicture: {ex.Message}");
                return StatusCode(500, "An error occurred while uploading the profile picture.");
            }
        }

        public IActionResult OnPost()
        {
            // Fetch the current logged-in user's email
            string? currentUserEmail = HttpContext.Session.GetString("Email");

            if (string.IsNullOrEmpty(currentUserEmail))
            {
                // Handle the case where the user isn't logged in; 
                Response.Redirect("/Index");
            }

            // Fetch the user from the database
            var user = _context.Users.FirstOrDefault(u => u.Email == currentUserEmail);

            if (user == null)
            {
                return RedirectToPage("/Error"); // Handle cases where the user is not found by using error page
            }

            // Update user information with the posted data
            user.Address1 = Address1;
            user.Address2 = Address2;
            user.City = City;
            user.State = State;

            _context.Users.Update(user);
            _context.SaveChanges();

            // Reload the page with updated information
            return RedirectToPage();
        }
    }
}
