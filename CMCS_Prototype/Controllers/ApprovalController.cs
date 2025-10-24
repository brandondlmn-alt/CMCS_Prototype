using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS_Prototype.Data;
using CMCS_Prototype.Models;

namespace CMCS_Prototype.Controllers
{
    public class ApprovalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ApprovalController> _logger;

        public ApprovalController(ApplicationDbContext context, ILogger<ApprovalController> logger)
        {
            _context = context;
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

        // Coordinator Dashboard - Pending Claims
        public async Task<IActionResult> CoordinatorDashboard()
        {
            if (!IsAuthenticated() || GetUserRole() != "Coordinator")
            {
                TempData["Error"] = "Access denied. Please login as a Coordinator.";
                return RedirectToAction("Login", "Account");
            }

            var pendingClaims = await _context.Claims
                .Include(c => c.Lecturer)
                    .ThenInclude(l => l.User)
                .Include(c => c.Documents)
                .Where(c => c.Status == "Pending")
                .OrderBy(c => c.SubmissionDate)
                .ToListAsync();

            return View(pendingClaims);
        }

        // Manager Dashboard - Reviewed Claims
        public async Task<IActionResult> ManagerDashboard()
        {
            if (!IsAuthenticated() || GetUserRole() != "Manager")
            {
                TempData["Error"] = "Access denied. Please login as a Manager.";
                return RedirectToAction("Login", "Account");
            }

            var reviewedClaims = await _context.Claims
                .Include(c => c.Lecturer)
                    .ThenInclude(l => l.User)
                .Include(c => c.Coordinator)
                    .ThenInclude(c => c.User)
                .Include(c => c.Documents)
                .Where(c => c.Status == "Reviewed")
                .OrderBy(c => c.SubmissionDate)
                .ToListAsync();

            return View(reviewedClaims);
        }

        public async Task<IActionResult> Dashboard()
        {
            var role = GetUserRole();
            return role?.ToLower() switch
            {
                "coordinator" => RedirectToAction("CoordinatorDashboard"),
                "manager" => RedirectToAction("ManagerDashboard"),
                _ => RedirectToAction("Login", "Account")
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CoordinatorReview(int claimId, string action, string notes)
        {
            if (!IsAuthenticated() || GetUserRole() != "Coordinator")
            {
                TempData["Error"] = "Access denied.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var claim = await _context.Claims.FindAsync(claimId);
                if (claim != null)
                {
                    var coordinator = await _context.Coordinators
                        .FirstOrDefaultAsync(c => c.UserID == GetUserId());

                    claim.CoordinatorReview = notes;
                    claim.Status = action == "approve" ? "Reviewed" : "Rejected";
                    claim.CoordinatorID = coordinator?.CoordinatorID;

                    await _context.SaveChangesAsync();
                    TempData["Message"] = $"Claim {action}d successfully";
                }
                else
                {
                    TempData["Error"] = "Claim not found";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during coordinator review");
                TempData["Error"] = "Error processing request";
            }

            return RedirectToAction("CoordinatorDashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManagerApprove(int claimId, string action, string notes)
        {
            if (!IsAuthenticated() || GetUserRole() != "Manager")
            {
                TempData["Error"] = "Access denied.";
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var claim = await _context.Claims.FindAsync(claimId);
                if (claim != null)
                {
                    var manager = await _context.Managers
                        .FirstOrDefaultAsync(m => m.UserID == GetUserId());

                    claim.ManagerApproval = notes;
                    claim.Status = action == "approve" ? "Approved" : "Rejected";
                    claim.ManagerID = manager?.ManagerID;

                    await _context.SaveChangesAsync();
                    TempData["Message"] = $"Claim {action}d successfully";
                }
                else
                {
                    TempData["Error"] = "Claim not found";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during manager approval");
                TempData["Error"] = "Error processing request";
            }

            return RedirectToAction("ManagerDashboard");
        }
    }
}