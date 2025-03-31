using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GitGodsLMS.Data;
using GitGodsLMS.Pages.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using static System.Collections.Specialized.BitVector32;
using System.Text.Json;

namespace GitGodsLMS.Pages.Class_Management
{
    public class EditModel : PageModel
    {
        private readonly GitGodsLMS.Data.LMSPagesContext _context;

        public EditModel(GitGodsLMS.Data.LMSPagesContext context)
        {
            _context = context;
        }

        public List<Class> UserClasses { get; set; }

        [BindProperty]
        public Class Class { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var section =  await _context.Classes.FirstOrDefaultAsync(m => m.Id == id);
            if (section == null)
            {
                return NotFound();
            }

            Class = section;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //var existingClass = await _context.Classes.FindAsync(Class.Id);

            //if (existingClass == null)
            //{
            //    return NotFound();
            //}

            //_context.Entry(existingClass).CurrentValues.SetValues(Class);

            //_context.Attach(Class).State = EntityState.Modified;

            var meetingDays = new List<string>();

            if (Class.Sunday) meetingDays.Add("Sunday");
            if (Class.Monday) meetingDays.Add("Monday");
            if (Class.Tuesday) meetingDays.Add("Tuesday");
            if (Class.Wednesday) meetingDays.Add("Wednesday");
            if (Class.Thursday) meetingDays.Add("Thursday");
            if (Class.Friday) meetingDays.Add("Friday");
            if (Class.Saturday) meetingDays.Add("Saturday");

            // Join the selected days with commas
            Class.MeetingTimes = string.Join(", ", meetingDays);

            // Append start and end times
            Class.MeetingTimes += $" {Class.StartTime} - {Class.EndTime}";

            _context.Classes.Update(Class);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassExists(Class.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var userClassesJson = HttpContext.Session.GetString("UserClasses");

            if (!string.IsNullOrEmpty(userClassesJson))
            {
                UserClasses = JsonSerializer.Deserialize<List<Class>>(userClassesJson);

                var classToEdit = UserClasses.FirstOrDefault(c => c.Id == Class.Id);
                if (classToEdit != null)
                {
                    classToEdit.Name = Class.Name;
                    classToEdit.Department = Class.Department;
                    classToEdit.CreditHours = Class.CreditHours;
                    classToEdit.CourseNumber = Class.CourseNumber;
                    classToEdit.Capacity = Class.Capacity;
                    classToEdit.Location = Class.Location;
                    classToEdit.Sunday = Class.Sunday;
                    classToEdit.Monday = Class.Monday;
                    classToEdit.Tuesday = Class.Tuesday;
                    classToEdit.Wednesday = Class.Wednesday;
                    classToEdit.Thursday = Class.Thursday;
                    classToEdit.Friday = Class.Friday;
                    classToEdit.Saturday = Class.Saturday;
                    classToEdit.StartTime = Class.StartTime;
                    classToEdit.EndTime = Class.EndTime;

                    HttpContext.Session.SetString("UserClasses", JsonSerializer.Serialize(UserClasses));
                }
            }

            return RedirectToPage("../Classes");
        }

        private bool ClassExists(int id)
        {
            return _context.Classes.Any(e => e.Id == id);
        }
    }
}
