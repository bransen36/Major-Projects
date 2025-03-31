using System;

namespace GitGodsLMS.Pages.Model
{
    public class AssignmentSubmission
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int StudentID { get; set; }
        public string? TextSubmission { get; set; }
        public string? AssignmentSubmissionPath { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        public int? PointsAwarded { get; set; }
    }
}
