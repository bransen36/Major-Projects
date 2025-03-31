using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GitGodsLMS.Data;
using GitGodsLMS.Pages.Assignment_Management;
using GitGodsLMS.Pages.Model;

namespace GitGodsLMSUnitTest
{
    [TestClass]
    public sealed class DeleteAssignmentTest
    {
        private LMSPagesContext _context;

        [TestInitialize]
        public void Initialize()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            string connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<LMSPagesContext>();
            optionsBuilder.UseSqlServer(connectionString);
            _context = new LMSPagesContext(optionsBuilder.Options);
        }

        [TestMethod]
        public async Task DeleteAssignment_ShouldRemoveAssignment()
        {
            // Arrange: Create a new test assignment
            var newAssignment = new Assignment
            {
                Title = "Test Assignment",
                Description = "Test Description",
                ClassId = 1,
                DueDateTime = DateTime.UtcNow.AddDays(7),
                CreationDateTime = DateTime.UtcNow,
                SubmissionType = SubmissionType.Text,
                MaxPoints = 100
            };

            _context.Assignments.Add(newAssignment);
            await _context.SaveChangesAsync();

            // Retrieve the created assignment
            var assignment = _context.Assignments.FirstOrDefault(a => a.Title == "Test Assignment");
            Assert.IsNotNull(assignment, "Assignment should have been created.");

            // Act: Delete the assignment
            var deleteModel = new DeleteModel(_context);
            await deleteModel.OnPostAsync(assignment.Id);

            // Assert: Ensure the assignment was deleted
            var deletedAssignment = _context.Assignments.FirstOrDefault(a => a.Id == assignment.Id);
            Assert.IsNull(deletedAssignment, "Assignment should have been deleted.");
        }
    }
}
