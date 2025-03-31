using GitGodsLMS.Data;
using GitGodsLMS.Pages.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GitGodsLMS.Pages
{
    public class AssignmentDetailsModel : PageModel
    {
        private readonly LMSPagesContext _context;
        private readonly IWebHostEnvironment _environment;

        public AssignmentDetailsModel(LMSPagesContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public Assignment Assignment { get; set; }

        [BindProperty]
        public string TextSubmission { get; set; }

        [BindProperty]
        public IFormFile AssignmentSubmissionFile { get; set; }

        public async Task<IActionResult> OnGetAsync(int assignmentId)
        {
            Assignment = await _context.Assignments
                .Include(a => a.Class)
                .FirstOrDefaultAsync(a => a.Id == assignmentId);

            if (Assignment == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostTextAsync(int assignmentId)
        {
            // Retrieve the assignment
            Assignment = await _context.Assignments.FindAsync(assignmentId);
            if (Assignment == null)
            {
                return NotFound();
            }

            // Validate that the text submission is not empty.
            if (string.IsNullOrWhiteSpace(TextSubmission))
            {
                ModelState.AddModelError("TextSubmission", "Submission cannot be empty.");
                return Page();
            }

            // Retrieve the current user from the session
            var email = HttpContext.Session.GetString("Email");
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            // Define the uploads folder and ensure it exists
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "assignmentsubmissions");
            Directory.CreateDirectory(uploadsFolder);

            // Create a unique file name with .txt extension
            var fileName = $"{user.Id}_{Assignment.Id}_{DateTime.Now:yyyyMMddHHmmss}.txt";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Write the text submission to the file
            await System.IO.File.WriteAllTextAsync(filePath, TextSubmission);

            // Create a relative path to store in the database
            var relativePath = $"/assignmentsubmissions/{fileName}";

            // Create a new submission record with the file path
            AssignmentSubmission submission = new AssignmentSubmission
            {
                AssignmentId = Assignment.Id,
                StudentID = user.Id,
                AssignmentSubmissionPath = relativePath,
                SubmittedAt = DateTime.Now
            };

            _context.AssignmentSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            // Redirect back to the assignment details page, including the assignment ID
            return RedirectToPage(new { assignmentId = Assignment.Id });
        }


        public async Task<IActionResult> OnPostFileAsync(int assignmentId)
        {
            Assignment = await _context.Assignments.FindAsync(assignmentId);
            if (Assignment == null)
            {
                return NotFound();
            }

            if (AssignmentSubmissionFile == null || AssignmentSubmissionFile.Length == 0)
            {
                ModelState.AddModelError("AssignmentSubmissionFile", "Please select a file to upload.");
                return Page();
            }

            var email = HttpContext.Session.GetString("Email");
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            // Save file to wwwroot/assignmentsubmissions folder.
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "assignmentsubmissions");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = $"{user.Id}_{Path.GetFileName(AssignmentSubmissionFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await AssignmentSubmissionFile.CopyToAsync(fileStream);
            }

            // Create a relative path to save in the database.
            var relativePath = $"/assignmentsubmissions/{fileName}";

            AssignmentSubmission submission = new AssignmentSubmission
            {
                AssignmentId = Assignment.Id,
                StudentID = user.Id,
                AssignmentSubmissionPath = relativePath,
                SubmittedAt = DateTime.Now
            };

            _context.AssignmentSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { assignmentId = Assignment.Id });
        }
    }
}
