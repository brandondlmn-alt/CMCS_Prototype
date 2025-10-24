using System.Collections.Generic;

namespace CMCS_Prototype.Models
{
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Lecturer> Lecturers { get; set; }
    }
}
