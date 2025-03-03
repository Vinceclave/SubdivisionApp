using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SubdivisionApp.Controllers.Dashboard
{
    public class DashboardController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin" || userRole == "Customer")
            {
                ViewData["Page"] = "Dashboard"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("Index");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}
