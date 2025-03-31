using System.Diagnostics.CodeAnalysis;

namespace GitGodsLMS.Pages.Model
{
    /// <summary>
    /// Represents a user within the LMS
    /// Contains their info and auth details
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets a unique ID for the user
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the email of the user
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password of the user
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name of the user
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name of the user
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the birthdate of the user.
        /// </summary>
        public string Birthdate { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether or not a user is a professor
        /// </summary>
        public bool isProfessor { get; set; } = false;

        /// <summary>
        /// Gets or sets the path for the uploaded profile picture
        /// </summary>
        public string? ProfilePicturePath { get; set; }

        /// <summary>
        /// Gets or sets the address of the user.
        /// </summary>
        public string? Address1 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the address of the user.
        /// </summary>
        public string? Address2 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city of the user.
        /// </summary>
        public string? City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the state of the user.
        /// </summary>
        public string? State { get; set; } = string.Empty;
    }
}
