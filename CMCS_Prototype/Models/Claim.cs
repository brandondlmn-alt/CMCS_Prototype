using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace CMCS_Prototype.Models
{
    public class Claim
    {
        public int ClaimID { get; set; }

        [Required]
        public string Month { get; set; }

        [Required]
        [Range(2020, 2030)]
        public int Year { get; set; }

        [Required]
        [Range(1, 200)]
        public decimal HoursWorked { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime SubmissionDate { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string Notes { get; set; }

        public string CoordinatorReview { get; set; }

        public string ManagerApproval { get; set; }

        // Foreign Keys
        public int LecturerID { get; set; }
        public int? CoordinatorID { get; set; }
        public int? ManagerID { get; set; }

        // Navigation Properties
        public Lecturer Lecturer { get; set; }
        public Coordinator Coordinator { get; set; }
        public Manager Manager { get; set; }
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}