using System;
using System.ComponentModel.DataAnnotations;

namespace CMCS_Prototype.Models
{
    public class Document
    {
        public int DocumentID { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FilePath { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.Now;

        [Required]
        public string DocumentType { get; set; }

        public long FileSize { get; set; }

        // Foreign Key
        public int ClaimID { get; set; }
        public Claim Claim { get; set; }
    }
}