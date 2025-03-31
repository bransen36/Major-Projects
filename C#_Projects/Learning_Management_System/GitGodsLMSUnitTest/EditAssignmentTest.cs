using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using GitGodsLMS.Data;
using GitGodsLMS.Pages.Model;

namespace GitGodsLMSUnitTest
{
    [TestClass]
    public sealed class EditAssignmentTest
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
        public void EditAssignment_ShouldUpdateAssignment()
        {
            // Arrange: Create a new test assignment
            var newAssignment = new Assignment
            {
                Title = "Original Title",
                Description = "Original Description",
                ClassId = 1,
                DueDateTime = DateTime.UtcNow.AddDays(7),
                CreationDateTime = DateTime.UtcNow,
                SubmissionType = SubmissionType.Text,
                MaxPoints = 100
            };

            _context.Assignments.Add(newAssignment);
            _context.SaveChanges();

            // Retrieve the created assignment
            var assignment = _context.Assignments.FirstOrDefault(a => a.Title == "Original Title");
            Assert.IsNotNull(assignment, "Assignment should have been created.");

            // Act: Modify the assignment and save changes
            assignment.Title = "Updated Title";
            assignment.Description = "Updated Description";
            _context.Assignments.Update(assignment);
            _context.SaveChanges();

            // Retrieve the updated assignment
            var updatedAssignment = _context.Assignments.FirstOrDefault(a => a.Id == assignment.Id);

            // Assert: Ensure changes were saved correctly
            Assert.IsNotNull(updatedAssignment, "Assignment should still exist.");
            Assert.AreEqual("Updated Title", updatedAssignment.Title, "Assignment title was not updated correctly.");
            Assert.AreEqual("Updated Description", updatedAssignment.Description, "Assignment description was not updated correctly.");
        }
    }
}
