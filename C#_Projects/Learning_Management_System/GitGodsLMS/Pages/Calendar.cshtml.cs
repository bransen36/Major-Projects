using GitGodsLMS.Data;
using GitGodsLMS.Pages.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitGodsLMS.Pages
{
    public class CalendarModel : PageModel
    {
        private readonly LMSPagesContext _context;
        public CalendarModel(LMSPagesContext context)
        {
            _context = context;
        }

        public List<object> CalendarEvents { get; set; } = new List<object>();

        private DateTime CombineDateAndTime(DateTime date, TimeOnly time)
        {
            return date.Date.Add(time.ToTimeSpan());
        }

        private List<DateTime> GetClassDatesForMonth(Class classInfo, DateTime monthStart, DateTime monthEnd)
        {
            var dates = new List<DateTime>();
            var currentDate = monthStart;

            while (currentDate <= monthEnd)
            {
                if ((classInfo.Sunday && currentDate.DayOfWeek == DayOfWeek.Sunday) ||
                    (classInfo.Monday && currentDate.DayOfWeek == DayOfWeek.Monday) ||
                    (classInfo.Tuesday && currentDate.DayOfWeek == DayOfWeek.Tuesday) ||
                    (classInfo.Wednesday && currentDate.DayOfWeek == DayOfWeek.Wednesday) ||
                    (classInfo.Thursday && currentDate.DayOfWeek == DayOfWeek.Thursday) ||
                    (classInfo.Friday && currentDate.DayOfWeek == DayOfWeek.Friday) ||
                    (classInfo.Saturday && currentDate.DayOfWeek == DayOfWeek.Saturday))
                {
                    dates.Add(currentDate);
                }
                currentDate = currentDate.AddDays(1);
            }

            return dates;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Index");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return RedirectToPage("/Index");
            }

            try
            {
                // Get current month's start and end dates
                var today = DateTime.Today;
                var monthStart = new DateTime(today.Year, today.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                if (user.isProfessor)
                {
                    // [Keep your existing professor classes code here]
                    var professorClasses = await _context.Classes
                        .Where(c => c.ProfessorId == user.Id)
                        .ToListAsync();

                    foreach (var classInfo in professorClasses)
                    {
                        var classDates = GetClassDatesForMonth(classInfo, monthStart, monthEnd);
                        foreach (var date in classDates)
                        {
                            CalendarEvents.Add(new
                            {
                                title = classInfo.Name,
                                start = CombineDateAndTime(date, classInfo.StartTime).ToString("o"),
                                end = CombineDateAndTime(date, classInfo.EndTime).ToString("o")
                            });
                        }
                    }

                    // Add assignments for professor's classes
                    var classIds = professorClasses.Select(c => c.Id).ToList();
                    var assignments = await _context.Assignments
                        .Where(a => classIds.Contains(a.ClassId))
                        .ToListAsync();

                    foreach (var assignment in assignments)
                    {
                        var associatedClass = professorClasses.FirstOrDefault(c => c.Id == assignment.ClassId);
                        CalendarEvents.Add(new
                        {
                            title = $"Due: {assignment.Title} ({associatedClass?.Name})",
                            start = assignment.DueDateTime.ToString("o"),
                            backgroundColor = "#dc3545", // Red color for assignments
                            textColor = "#ffffff",
                            allDay = false,
                            url = Url.Page("/AssignmentDetails", new { assignmentId = assignment.Id }) // Add URL here
                        });
                    }
                }
                else
                {
                    // [Keep your existing student classes code here]
                    var studentClasses = await _context.StudentClasses
                        .Include(sc => sc.Class)
                        .Where(sc => sc.UserId == user.Id)
                        .ToListAsync();

                    foreach (var studentClass in studentClasses)
                    {
                        var classInfo = studentClass.Class;
                        if (classInfo != null)
                        {
                            var classDates = GetClassDatesForMonth(classInfo, monthStart, monthEnd);
                            foreach (var date in classDates)
                            {
                                CalendarEvents.Add(new
                                {
                                    title = classInfo.Name,
                                    start = CombineDateAndTime(date, classInfo.StartTime).ToString("o"),
                                    end = CombineDateAndTime(date, classInfo.EndTime).ToString("o")
                                });
                            }
                        }
                    }

                    // Add assignments for student's classes
                    var enrolledClassIds = studentClasses.Select(sc => sc.ClassId).ToList();
                    var assignments = await _context.Assignments
                        .Where(a => enrolledClassIds.Contains(a.ClassId))
                        .ToListAsync();

                    foreach (var assignment in assignments)
                    {
                        var associatedClass = studentClasses.FirstOrDefault(sc => sc.ClassId == assignment.ClassId)?.Class;
                        CalendarEvents.Add(new
                        {
                            title = $"Due: {assignment.Title} ({associatedClass?.Name})",
                            start = assignment.DueDateTime.ToString("o"),
                            backgroundColor = "#dc3545", // Red color for assignments
                            textColor = "#ffffff",
                            allDay = false,
                            url = Url.Page("/AssignmentDetails", new { assignmentId = assignment.Id }) // Add URL here
                        });
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error");
            }
        }
    }
}