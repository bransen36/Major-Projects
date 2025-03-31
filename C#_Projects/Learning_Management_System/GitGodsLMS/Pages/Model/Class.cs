namespace GitGodsLMS.Pages.Model
{
    public class Class
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public int CreditHours { get; set; }
        public int CourseNumber { get; set; }
        public int Capacity { get; set; }
        public string Location { get; set; }
        public bool Sunday { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        
        public string? MeetingTimes { get; set; }
        public int ProfessorId { get; set; }
    }
}
/** dummy change **/

