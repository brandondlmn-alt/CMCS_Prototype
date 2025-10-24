using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CMCS_Prototype.Models;

namespace CMCS_Prototype.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Redirect to login if not authenticated, otherwise to appropriate dashboard
            if (HttpContext.Session.GetString("UserID") != null)
            {
                var role = HttpContext.Session.GetString("Role");
                return RedirectToAction("Dashboard", GetDashboardController(role));
            }
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GetDashboardController(string role)
        {
            return role?.ToLower() switch
            {
                "lecturer" => "Lecturer",
                "coordinator" => "Approval",
                "manager" => "Approval",
                _ => "Home"
            };
        }
    }
}