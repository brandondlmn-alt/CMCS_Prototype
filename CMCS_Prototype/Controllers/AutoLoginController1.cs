using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace CMCS_Prototype.Controllers
{
    public class AutoLoginController : Controller
    {
        [Route("")]
        [Route("autologin")]
        [Route("autologin/{role?}")]
        public IActionResult Index(string role = "lecturer")
        {
            // Set session based on role
            switch (role?.ToLower())
            {
                case "coordinator":
                    HttpContext.Session.SetString("UserID", "2");
                    HttpContext.Session.SetString("Email", "coordinator@cmcs.edu");
                    HttpContext.Session.SetString("FirstName", "Sarah");
                    HttpContext.Session.SetString("LastName", "Johnson");
                    HttpContext.Session.SetString("Role", "coordinator");
                    HttpContext.Session.SetString("FullName", "Sarah Johnson");
                    break;
                case "manager":
                    HttpContext.Session.SetString("UserID", "3");
                    HttpContext.Session.SetString("Email", "manager@cmcs.edu");
                    HttpContext.Session.SetString("FirstName", "Michael");
                    HttpContext.Session.SetString("LastName", "Brown");
                    HttpContext.Session.SetString("Role", "manager");
                    HttpContext.Session.SetString("FullName", "Michael Brown");
                    break;
                default: // lecturer
                    HttpContext.Session.SetString("UserID", "1");
                    HttpContext.Session.SetString("Email", "lecturer@cmcs.edu");
                    HttpContext.Session.SetString("FirstName", "John");
                    HttpContext.Session.SetString("LastName", "Smith");
                    HttpContext.Session.SetString("Role", "lecturer");
                    HttpContext.Session.SetString("FullName", "John Smith");
                    break;
            }

            TempData["Success"] = $"Auto-logged in as {HttpContext.Session.GetString("Role")}";
            return RedirectToAction("Index", "Home");
        }
    }
}