namespace CMCS_Prototype.Models
{
    public class Module
    {
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public string ModuleCode { get; set; }
        public string Description { get; set; }
        public int CourseID { get; set; }

        public Course Course { get; set; }
    }
}