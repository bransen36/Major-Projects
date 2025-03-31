using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GitGodsLMS.Data;
using GitGodsLMS.Pages.Model;
using System.Linq;
using System.Threading.Tasks;

namespace GitGodsLMS.Pages
{
    public class AssignmentGradingModel : PageModel
    {
        private readonly LMSPagesContext _context;

        public AssignmentGradingModel(LMSPagesContext context)
        {
            _context = context;
        }

        // Bind the assignment ID from the query string.
        [BindProperty(SupportsGet = true)]
        public int? AssignmentId { get; set; }

        // List of submissions that have not yet been graded.
        public List<GradingItem> GradingItems { get; set; } = new List<GradingItem>();

        public void OnGet()
        {
            // Query ungraded submissions (PointsAwarded is null)
            var query =
                from submission in _context.AssignmentSubmissions
                join assignment in _context.Assignments on submission.AssignmentId equals assignment.Id
                join cls in _context.Classes on assignment.ClassId equals cls.Id
                join user in _context.Users on submission.StudentID equals user.Id
                where submission.PointsAwarded == null
                select new GradingItem
                {
                    SubmissionId = submission.Id,
                    AssignmentId = assignment.Id,
                    MaxPoints = assignment.MaxPoints,
                    StudentFullName = user.FirstName + " " + user.LastName,
                    ClassName = cls.Name,
                    SubmissionPath = submission.AssignmentSubmissionPath
                };

            // If an assignment ID was provided, filter the query accordingly.
            if (AssignmentId.HasValue)
            {
                query = query.Where(x => x.AssignmentId == AssignmentId.Value);
            }

            GradingItems = query.ToList();
        }

        // This handler grades a single submission.
        public async Task<IActionResult> OnPostGradeAsync(int SubmissionId, int PointsAwarded)
        {
            // Retrieve the submission record.
            var submission = await _context.AssignmentSubmissions.FindAsync(SubmissionId);
            if (submission == null)
            {
                return NotFound();
            }

            // Retrieve the corresponding assignment to validate max points.
            var assignment = await _context.Assignments.FindAsync(submission.AssignmentId);
            if (assignment == null)
            {
                return NotFound();
            }

            // Validate that the entered score does not exceed the maximum.
            if (PointsAwarded > assignment.MaxPoints)
            {
                ModelState.AddModelError("", $"Points cannot exceed maximum points of {assignment.MaxPoints}.");
                OnGet(); // Reload grading items.
                return Page();
            }

            // Save the grade.
            submission.PointsAwarded = PointsAwarded;
            await _context.SaveChangesAsync();

            // Redirect to refresh the page. The graded row will now be excluded.
            return RedirectToPage(new { assignmentId = AssignmentId });
        }

        public class GradingItem
        {
            public int SubmissionId { get; set; }
            public int AssignmentId { get; set; }
            public int MaxPoints { get; set; }
            public string StudentFullName { get; set; } = string.Empty;
            public string ClassName { get; set; } = string.Empty;
            public string SubmissionPath { get; set; } = string.Empty;
        }
    }
}
