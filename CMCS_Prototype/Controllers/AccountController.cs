using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CMCS_Prototype.Data;
using CMCS_Prototype.Models;

namespace CMCS_Prototype.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login - Simple role selection
        public IActionResult Login()
        {
            HttpContext.Session.Clear();
            return View();
        }

        // POST: /Account/LoginAs - Login by role selection
        [HttpPost]
        public IActionResult LoginAs(string role)
        {
            // Find a user with the selected role
            var user = _context.Users.FirstOrDefault(u => u.Role.ToLower() == role.ToLower());

            if (user != null)
            {
                // Set session
                HttpContext.Session.SetString("UserID", user.UserID.ToString());
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("FirstName", user.FirstName);
                HttpContext.Session.SetString("LastName", user.LastName);
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetString("FullName", $"{user.FirstName} {user.LastName}");

                TempData["Success"] = $"Welcome, {user.FirstName}! You are logged in as {user.Role}.";

                // Redirect to appropriate dashboard
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Error"] = $"No {role} account found.";
                return RedirectToAction("Login");
            }
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            var userName = HttpContext.Session.GetString("FullName");
            HttpContext.Session.Clear();
            TempData["Success"] = $"Goodbye, {userName}! You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        // GET: /Account/Profile - Show current user profile
        public IActionResult Profile()
        {
            if (HttpContext.Session.GetString("UserID") == null)
            {
                return RedirectToAction("Login");
            }

            var user = new User
            {
                UserID = int.Parse(HttpContext.Session.GetString("UserID")),
                FirstName = HttpContext.Session.GetString("FirstName"),
                LastName = HttpContext.Session.GetString("LastName"),
                Email = HttpContext.Session.GetString("Email"),
                Role = HttpContext.Session.GetString("Role")
            };

            return View(user);
        }
    }
}