using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Subdivision.Data;
using Subdivision.Models;
using System;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Subdivision.Controllers
{
    public class AuthController : Controller
    {
        private readonly SubdivisionDbContext _context;
        private const int MaxLoginAttempts = 5;

        public AuthController(SubdivisionDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn() => HttpContext.Session.GetInt32("UserId") != null;

        [Route("login")]
        [HttpGet]
        public IActionResult Login()
        {
            if (IsLoggedIn())
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            ViewData["Page"] = "Login";
            ViewData["UserType"] = "Guest";
            ViewData["HideNav"] = true;
            return View();
        }

        private IActionResult RedirectToRole(UserType userType)
        {
            switch (userType)
            {
                case UserType.Admin:
                    return RedirectToAction("Dashboard", "Admin");
                case UserType.Staff:
                    return RedirectToAction("Index", "Staff");
                case UserType.Homeowner:
                    return RedirectToAction("Index", "Homeowner");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }

        [Route("login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                int loginAttempts = HttpContext.Session.GetInt32("LoginAttempts") ?? 0;

                if (loginAttempts >= MaxLoginAttempts)
                {
                    TempData["Error"] = "Too many login attempts. Please try again later.";
                    return RedirectToAction("Login");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    HttpContext.Session.SetInt32("LoginAttempts", loginAttempts + 1);
                    TempData["Error"] = "Invalid username or password.";
                    return RedirectToAction("Login");
                }

                // Successful login
                HttpContext.Session.Clear();
                HttpContext.Session.SetInt32("UserId", user.LoginId);
                HttpContext.Session.SetInt32("UserType", (int)user.UserType);
                HttpContext.Session.SetString("Username", user.Username);

                // Set the appropriate ID based on user type
                if (user.UserType == UserType.Admin)
                {
                    HttpContext.Session.SetInt32("AdminId", user.LoginId);
                }
                else if (user.UserType == UserType.Staff)
                {
                    HttpContext.Session.SetInt32("StaffId", user.LoginId);
                }
                else if (user.UserType == UserType.Homeowner)
                {
                    HttpContext.Session.SetInt32("HomeownerId", user.LoginId);
                }

                return RedirectToRole(user.UserType);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Error"] = "An error occurred: " + ex.Message;
                return RedirectToAction("Login");
            }
        }

        [Route("logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            // Clear all session data
            HttpContext.Session.Clear();
            
            // Remove all cookies
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
