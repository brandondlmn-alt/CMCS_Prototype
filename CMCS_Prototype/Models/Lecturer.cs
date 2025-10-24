namespace CMCS_Prototype.Models
{
    public class Lecturer
    {
        public int LecturerID { get; set; }
        public int UserID { get; set; }
        public decimal HourlyRate { get; set; }
        public int RoleID { get; set; }

        public User User { get; set; }
    }
}