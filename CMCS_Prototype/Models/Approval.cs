namespace CMCS_Prototype.Models
{
    public class Approval
    {
        public int ApprovalID { get; set; }
        public int RequestID { get; set; }
        public int ApproverID { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public DateTime? ApprovalDate { get; set; }

        public TeachingRequest TeachingRequest { get; set; }
        public User Approver { get; set; }
    }
}