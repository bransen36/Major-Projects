using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GitGodsLMS.Pages.Model
{
    public enum SubmissionType
    {
        Text,
        File
    }

    public class Assignment
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int ClassId { get; set; }

        [Required]
        public DateTime DueDateTime { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreationDateTime { get; set; } = DateTime.UtcNow;

        [Required]
        public SubmissionType SubmissionType { get; set; }

        [Required]
        public int MaxPoints { get; set; }

        public Class? Class { get; set; }

    }
}
