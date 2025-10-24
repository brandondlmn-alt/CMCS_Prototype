namespace CMCS_Prototype.Models
{
    public class Coordinator
    {
        public int CoordinatorID { get; set; }
        public int UserID { get; set; }
        public string Department { get; set; }

        public User User { get; set; }
    }
}