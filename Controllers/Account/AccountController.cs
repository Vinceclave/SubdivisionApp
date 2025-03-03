using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SubdivisionApp.Data;
using SubdivisionApp.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using System;

namespace SubdivisionApp.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly SubdivisionDbContext _context;

        public AccountController(SubdivisionDbContext context)
        {
            _context = context;
        }

        // REGISTER GET
        public IActionResult Register()
        {
            ViewData["HideHeaderFooter"] = true;  // Hide header/footer
            return View();
        }

        // REGISTER POST
        [HttpPost]
        public IActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (_context.Users.Any(u => u.UserEmail == model.UserEmail))
                {
                    ModelState.AddModelError("UserEmail", "Email already exists.");
                    return View(model);
                }
                Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("admin123"));

                model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);  // Hash password
                model.DateCreated = DateTime.Now;  // Set the creation date
                model.UserRole = "Customer";  // Set default role

                _context.Users.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // LOGIN GET
        public IActionResult Login()
        {
            ViewData["HideHeaderFooter"] = true;  // Hide header/footer
            return View();
        }

        // LOGIN POST
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                HttpContext.Session.SetString("UserRole", user.UserRole);
                HttpContext.Session.SetInt32("UserId", user.Id);

                return RedirectToAction("Index", "Dashboard");  // Redirect to dynamic dashboard
            }

            ViewBag.Message = "Invalid credentials!";
            return View();
        }



        // LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        [Authorize]
        public IActionResult Index()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserRole = userRole;
            return View();
        }




    }
}
