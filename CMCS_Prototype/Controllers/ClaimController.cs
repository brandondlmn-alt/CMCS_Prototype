using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using CMCS_Prototype.Models;
using CMCS_Prototype.ViewModels;
using CMCS_Prototype.Data;

namespace CMCS_Prototype.Controllers
{
    public class ClaimController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ClaimController> _logger;

        public ClaimController(ApplicationDbContext context, IWebHostEnvironment environment, ILogger<ClaimController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        private bool IsAuthenticated()
        {
            return HttpContext.Session.GetString("UserID") != null;
        }

        private string GetUserRole()
        {
            return HttpContext.Session.GetString("Role") ?? "";
        }

        private int GetUserId()
        {
            return int.Parse(HttpContext.Session.GetString("UserID") ?? "0");
        }

        // GET: Submit new claim
        public IActionResult Submit()
        {
            if (!IsAuthenticated() || GetUserRole() != "Lecturer")
            {
                TempData["Error"] = "Access denied. Please login as a Lecturer.";
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: Submit new claim with file upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(ClaimViewModel model)
        {
            if (!IsAuthenticated() || GetUserRole() != "Lecturer")
            {
                TempData["Error"] = "Access denied.";
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = GetUserId();
                    var lecturer = await _context.Lecturers
                        .Include(l => l.User)
                        .FirstOrDefaultAsync(l => l.UserID == userId);

                    if (lecturer == null)
                    {
                        TempData["Error"] = "Lecturer profile not found.";
                        return RedirectToAction("Login", "Account");
                    }

                    var claim = new Claim
                    {
                        LecturerID = lecturer.LecturerID,
                        Month = model.Month,
                        Year = model.Year,
                        HoursWorked = model.HoursWorked,
                        TotalAmount = model.HoursWorked * lecturer.HourlyRate,
                        Notes = model.Notes,
                        Status = "Pending",
                        SubmissionDate = DateTime.Now
                    };

                    _context.Claims.Add(claim);
                    await _context.SaveChangesAsync();

                    // Handle document upload
                    if (model.SupportingDocument != null)
                    {
                        await UploadDocumentAsync(model.SupportingDocument, claim.ClaimID);
                    }

                    TempData["Success"] = "Claim submitted successfully!";
                    return RedirectToAction("Dashboard", "Lecturer");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error submitting claim");
                    ModelState.AddModelError("", "Error submitting claim: " + ex.Message);
                }
            }
            return View(model);
        }

        // GET: Claim details
        public async Task<IActionResult> Details(int id)
        {
            if (!IsAuthenticated())
            {
                TempData["Error"] = "Please login to view claim details.";
                return RedirectToAction("Login", "Account");
            }

            var claim = await _context.Claims
                .Include(c => c.Lecturer)
                    .ThenInclude(l => l.User)
                .Include(c => c.Coordinator)
                    .ThenInclude(c => c.User)
                .Include(c => c.Manager)
                    .ThenInclude(m => m.User)
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.ClaimID == id);

            if (claim == null)
            {
                return NotFound();
            }

            // Check authorization
            var userRole = GetUserRole();
            var userId = GetUserId();

            if (userRole == "Lecturer")
            {
                var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.UserID == userId);
                if (lecturer?.LecturerID != claim.LecturerID)
                {
                    TempData["Error"] = "Access denied.";
                    return RedirectToAction("Dashboard", "Lecturer");
                }
            }

            return View(claim);
        }

        // GET: All claims for lecturer
        public async Task<IActionResult> MyClaims()
        {
            if (!IsAuthenticated() || GetUserRole() != "Lecturer")
            {
                TempData["Error"] = "Access denied. Please login as a Lecturer.";
                return RedirectToAction("Login", "Account");
            }

            var userId = GetUserId();
            var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.UserID == userId);

            if (lecturer == null)
            {
                TempData["Error"] = "Lecturer profile not found.";
                return RedirectToAction("Login", "Account");
            }

            var claims = await _context.Claims
                .Where(c => c.LecturerID == lecturer.LecturerID)
                .OrderByDescending(c => c.SubmissionDate)
                .ToListAsync();

            return View(claims);
        }

        private async Task UploadDocumentAsync(IFormFile file, int claimId)
        {
            // Validate file
            if (file.Length > 5 * 1024 * 1024) // 5MB limit
            {
                throw new ValidationException("File size must be less than 5MB");
            }

            var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ValidationException("Only PDF, DOCX, and XLSX files are allowed");
            }

            // Save file
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Save document record
            var document = new Document
            {
                FileName = file.FileName,
                FilePath = uniqueFileName,
                DocumentType = fileExtension,
                FileSize = file.Length,
                UploadDate = DateTime.Now,
                ClaimID = claimId
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
        }

        // API endpoint for getting pending claims count (for notifications)
        [HttpGet]
        public async Task<IActionResult> GetPendingClaimsCount()
        {
            if (!IsAuthenticated())
            {
                return Json(new { count = 0 });
            }

            var userRole = GetUserRole();
            var userId = GetUserId();

            int count = 0;

            if (userRole == "Lecturer")
            {
                var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.UserID == userId);
                if (lecturer != null)
                {
                    count = await _context.Claims
                        .Where(c => c.LecturerID == lecturer.LecturerID && c.Status == "Pending")
                        .CountAsync();
                }
            }
            else if (userRole == "Coordinator")
            {
                count = await _context.Claims
                    .Where(c => c.Status == "Pending")
                    .CountAsync();
            }
            else if (userRole == "Manager")
            {
                count = await _context.Claims
                    .Where(c => c.Status == "Reviewed")
                    .CountAsync();
            }

            return Json(new { count });
        }
    }
}
