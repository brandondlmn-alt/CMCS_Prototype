using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using CMCS_Prototype.CustomValidationAttributes;

namespace CMCS_Prototype.ViewModels
{
    public class ClaimViewModel
    {
        [Required(ErrorMessage = "Month is required")]
        [Display(Name = "Month")]
        public string Month { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Range(2020, 2030, ErrorMessage = "Year must be between 2020 and 2030")]
        [Display(Name = "Year")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Hours worked is required")]
        [Range(1, 200, ErrorMessage = "Hours worked must be between 1 and 200")]
        [Display(Name = "Hours Worked")]
        public decimal HoursWorked { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        [Display(Name = "Additional Notes")]
        public string Notes { get; set; }

        [FileExtensions(Extensions = ".pdf,.docx,.xlsx", ErrorMessage = "Only PDF, DOCX, and XLSX files are allowed")]
        [MaxFileSize(5 * 1024 * 1024, ErrorMessage = "File size cannot exceed 5MB")]
        [Display(Name = "Supporting Document")]
        public IFormFile SupportingDocument { get; set; }
    }}