namespace CMCS_Prototype.Models
{
    public class TeachingRequest
    {
        public int RequestID { get; set; }
        public int LecturerID { get; set; }
        public int ModuleID { get; set; }
        public string Status { get; set; }
        public int Hours { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public Lecturer Lecturer { get; set; }
        public Module Module { get; set; }
    }
}