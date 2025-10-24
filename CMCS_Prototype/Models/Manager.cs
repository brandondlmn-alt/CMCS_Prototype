namespace CMCS_Prototype.Models
{
    public class Manager
    {
        public int ManagerID { get; set; }
        public int UserID { get; set; }
        public string Department { get; set; }

        // Navigation property
        public User User { get; set; }
    }
}