using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SubdivisionApp.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
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

        public IActionResult Community()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                ViewData["Page"] = "Community"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("Community");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Security()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                ViewData["Page"] = "Security"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("Security");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Visitor()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                ViewData["Page"] = "Visitor"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("Visitor");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Vehicle()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                ViewData["Page"] = "Vehicle"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("Vehicle");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

         public IActionResult Contacts()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                ViewData["Page"] = "Contacts"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("Contacts");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

         public IActionResult Reservation()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                ViewData["Page"] = "Reservation"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("Reservation");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Settings()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                ViewData["Page"] = "Settings"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("Settings");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Profile()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                ViewData["Page"] = "Profile"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("Profile");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Services()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                ViewData["Page"] = "Services"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("Services");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult User_Management()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                ViewData["Page"] = "User_Management"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("User_Management");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

          public IActionResult Billing()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                ViewData["Page"] = "Billing"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("Billing");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        //Temporary Security-staff page controller
        // ang index sa staff or dashboard parehas ra sa dashboard sa homeowner.
        public IActionResult Security_Profile()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                ViewData["Page"] = "Security_Profile"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("Security_Profile");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Security_Contacts()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                ViewData["Page"] = "Security_Contacts"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("Security_Contacts");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Security_Settings()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                ViewData["Page"] = "Security_Settings"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("Security_Settings");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Security_Services()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                ViewData["Page"] = "Security_Services"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("Security_Services");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Security_Vehicle()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                ViewData["Page"] = "Security_Vehicle"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("Security_Vehicle");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Security_Visitor()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                ViewData["Page"] = "Security_Visitor"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("Security_Contacts");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Security_Visitor_Logs()
        {
            string? userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole == "Admin")
            {
                ViewData["Page"] = "Security_Visitor_Logs"; // This will hide header and footer
                ViewBag.UserRole = userRole;
                return View("Security_Visitor_Logs");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

    }
}
