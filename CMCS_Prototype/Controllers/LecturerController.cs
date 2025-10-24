using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS_Prototype.Data;
using CMCS_Prototype.Models;

namespace CMCS_Prototype.Controllers
{
    public class LecturerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LecturerController(ApplicationDbContext context)
        {
            _context = context;
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

        public async Task<IActionResult> Dashboard()
        {
            if (!IsAuthenticated() || GetUserRole() != "Lecturer")
            {
                TempData["Error"] = "Access denied. Please login as a Lecturer.";
                return RedirectToAction("Login", "Account");
            }

            var userId = GetUserId();
            var lecturer = await _context.Lecturers
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.UserID == userId);

            if (lecturer == null)
            {
                TempData["Error"] = "Lecturer profile not found.";
                return RedirectToAction("Login", "Account");
            }

            var claims = await _context.Claims
                .Where(c => c.LecturerID == lecturer.LecturerID)
                .OrderByDescending(c => c.SubmissionDate)
                .Take(10)
                .ToListAsync();

            ViewBag.LecturerName = $"{lecturer.User.FirstName} {lecturer.User.LastName}";
            ViewBag.LecturerId = lecturer.LecturerID;

            return View(claims);
        }
    }
}