using GitGodsLMS.Pages.Model;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GitGodsLMS.Data
{
    public class LMSPagesContext : DbContext
    {
        public LMSPagesContext(DbContextOptions<LMSPagesContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Class> Classes { get; set; } = default!;
        public DbSet<StudentClass> StudentClasses { get; set; } = default!;

        public DbSet<Assignment> Assignments { get; set; } = default!;
        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; } = default!;
    }
}
