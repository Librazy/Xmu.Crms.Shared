namespace Xmu.Crms.Shared.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public Student Student { get; set; }
        public ClassInfo ClassInfo { get; set; }
        public int AttendanceSatus { get; set; }
    }
}