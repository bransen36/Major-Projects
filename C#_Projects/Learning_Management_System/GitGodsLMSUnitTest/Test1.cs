using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using GitGodsLMS.Data;
using GitGodsLMS.Pages.Model;

namespace GitGodsLMSUnitTest
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void InstructorCourseCreationTest()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            string connectionString = config.GetConnectionString("DefaultConnection");

            DbContextOptions<LMSPagesContext> options = new DbContextOptions<LMSPagesContext>();
            DbContextOptionsBuilder<LMSPagesContext> builder = new DbContextOptionsBuilder<LMSPagesContext>(options);
            SqlServerDbContextOptionsExtensions.UseSqlServer(builder, connectionString, null);
            var _context = new LMSPagesContext(builder.Options);

            var professor = new User
            {
                Email = "professor@test.com",
                isProfessor = true,
                FirstName = "John",
                LastName = "Doe"
            };
            _context.Users.Add(professor);
            _context.SaveChanges();

            var newClass = new Class
            {
                Name = "Unit Test Class",
                Department = "CS",
                CreditHours = 3,
                CourseNumber = 101,
                Capacity = 30,
                Location = "Test Room",
                ProfessorId = professor.Id,
                Sunday = false,
                Monday = true,
                Tuesday = false,
                Wednesday = true,
                Thursday = false,
                Friday = true,
                Saturday = false,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(10, 0),
                MeetingTimes = "Monday, Wednesday, Friday 9:00-10:00"
            };
            _context.Classes.Add(newClass);
            _context.SaveChanges();

            var createdClass = _context.Classes.FirstOrDefault(c => c.Name == "Unit Test Class");
            Assert.IsNotNull(createdClass, "The class should have been created.");
            Assert.AreEqual("Unit Test Class", createdClass.Name, "The class name is not as expected.");
            Assert.AreEqual(professor.Id, createdClass.ProfessorId, "The ProfessorId is not correct.");
        }
    }
}
