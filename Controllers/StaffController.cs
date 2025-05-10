using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Subdivision.Models;
using Subdivision.Data;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Subdivision.Controllers
{
    public class StaffController : Controller
    {
        private readonly SubdivisionDbContext _context;

        public StaffController(SubdivisionDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn() => HttpContext.Session.GetInt32("UserId") != null;
        private bool IsStaff() => HttpContext.Session.GetInt32("UserType") == (int)UserType.Staff;

        public IActionResult Index()
        {
            if (!IsLoggedIn() || !IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["Page"] = "Index";
            ViewData["UserType"] = "Staff";
            return View();
        }

        // Security Staff Functions
        public IActionResult VisitorPasses()
        {
            if (!IsLoggedIn() || !IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["Page"] = "VisitorPasses";
            ViewData["UserType"] = "Staff";
            return View();
        }

        public IActionResult SecurityIncidents()
        {
            if (!IsLoggedIn() || !IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["Page"] = "SecurityIncidents";
            ViewData["UserType"] = "Staff";
            return View();
        }

        // Maintenance Staff Functions
        public IActionResult MaintenanceTasks()
        {
            if (!IsLoggedIn() || !IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["Page"] = "MaintenanceTasks";
            ViewData["UserType"] = "Staff";
            return View();
        }

        public IActionResult ServiceRequests()
        {
            if (!IsLoggedIn() || !IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["Page"] = "ServiceRequests";
            ViewData["UserType"] = "Staff";
            return View();
        }

        // Housekeeping Staff Functions
        public IActionResult CleaningSchedule()
        {
            if (!IsLoggedIn() || !IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["Page"] = "CleaningSchedule";
            ViewData["UserType"] = "Staff";
            return View();
        }

        public IActionResult Inventory()
        {
            if (!IsLoggedIn() || !IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["Page"] = "Inventory";
            ViewData["UserType"] = "Staff";
            return View();
        }

        // Common Staff Functions
        public async Task<IActionResult> Profile()
        {
            if (!IsLoggedIn() || !IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _context.Users
                .Include(u => u.Staff)
                    .ThenInclude(s => s.Forums.Where(f => !string.IsNullOrEmpty(f.ImagePath)))
                .FirstOrDefaultAsync(u => u.LoginId == userId.Value);

            if (user == null)
            {
                return NotFound();
            }

            ViewData["Page"] = "Profile";
            ViewData["UserType"] = "Staff";
            return View(user);
        }

        [HttpPost]
        [Route("staff/profile/update")]
        public async Task<IActionResult> UpdateProfile([FromForm] IFormFile? photo, [FromForm] string? removePhoto)
        {
            if (!IsLoggedIn() || !IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
            {
                return NotFound();
            }

            if (photo != null && photo.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(fileStream);
                }

                if (!string.IsNullOrEmpty(user.Image) && user.Image != "/images/default-avatar.png")
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.Image.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                user.Image = "/uploads/profiles/" + uniqueFileName;
            }
            else if (removePhoto == "true" && !string.IsNullOrEmpty(user.Image) && user.Image != "/images/default-avatar.png")
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.Image.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                user.Image = "";
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Profile));
        }

        public IActionResult Reports()
        {
            if (!IsLoggedIn() || !IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["Page"] = "Reports";
            ViewData["UserType"] = "Staff";
            return View();
        }

        public async Task<IActionResult> Settings()
        {
            if (!IsLoggedIn() || !IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _context.Users
                .Include(u => u.Staff)
                .FirstOrDefaultAsync(u => u.LoginId == userId.Value);
                
            if (user == null)
            {
                return NotFound();
            }

            ViewData["Page"] = "Settings";
            ViewData["UserType"] = "Staff";
            return View(user);
        }

        [HttpPost]
        [Route("staff/settings/update")]
        public async Task<IActionResult> UpdateSettings([FromForm] User model, IFormFile? Photo, string newPassword, string confirmPassword)
        {
            if (!IsLoggedIn() || !IsStaff())
            {
                return Json(new { success = false, message = "Unauthorized access" });
            }

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                var user = await _context.Users.FindAsync(userId.Value);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                // Update user information
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address;

                // Handle photo upload
                if (Photo != null && Photo.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Photo.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Photo.CopyToAsync(fileStream);
                    }

                    if (!string.IsNullOrEmpty(user.Image) && user.Image != "/images/default-avatar.png")
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.Image.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    user.Image = "/uploads/profiles/" + uniqueFileName;
                }

                // Handle password change if provided
                if (!string.IsNullOrEmpty(newPassword))
                {
                    if (newPassword != confirmPassword)
                    {
                        return Json(new { success = false, message = "Passwords do not match" });
                    }
                    user.Password = HashPassword(newPassword);
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Settings updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating settings: " + ex.Message });
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        [HttpGet]
        [Route("staff/service-requests/list")]
        public async Task<IActionResult> GetServiceRequests()
        {
            if (!IsLoggedIn() || !IsStaff())
            {
                return Json(new { success = false, message = "Unauthorized access" });
            }

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.LoginId == userId.Value);
                if (staff == null)
                {
                    return Json(new { success = false, message = "Staff profile not found" });
                }

                // Only show assigned requests for this staff member
                var requests = await _context.ServiceRequests
                    .Where(sr => sr.StaffId == staff.StaffId)
                    .Include(sr => sr.Homeowner)
                        .ThenInclude(h => h.User)
                    .OrderByDescending(sr => sr.RequestDateTime)
                    .Select(sr => new
                    {
                        sr.ServiceRequestId,
                        sr.RequestType,
                        sr.Priority,
                        sr.Description,
                        sr.RequestDateTime,
                        sr.ServiceStatus,
                        HomeownerName = sr.Homeowner != null && sr.Homeowner.User != null ? 
                            sr.Homeowner.User.FirstName + " " + sr.Homeowner.User.LastName : ""
                    })
                    .ToListAsync();

                return Json(new { success = true, requests });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPut]
        [Route("staff/service-requests/{id}/update-status")]
        public async Task<IActionResult> UpdateServiceRequestStatus(int id, [FromBody] JsonElement data)
        {
            if (!IsLoggedIn() || !IsStaff())
            {
                return Json(new { success = false, message = "Unauthorized access" });
            }

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                var staff = await _context.Staffs
                    .FirstOrDefaultAsync(s => s.LoginId == userId.Value);

                if (staff == null)
                {
                    return Json(new { success = false, message = "Staff profile not found" });
                }

                var serviceRequest = await _context.ServiceRequests
                    .FirstOrDefaultAsync(sr => sr.ServiceRequestId == id && sr.StaffId == staff.StaffId);

                if (serviceRequest == null)
                {
                    return Json(new { success = false, message = "Service request not found or not assigned to you" });
                }

                // Properly parse the status from the JSON payload
                if (!data.TryGetProperty("status", out JsonElement statusElement) || 
                    statusElement.ValueKind != JsonValueKind.String)
                {
                    return Json(new { success = false, message = "Invalid status provided" });
                }

                string status = statusElement.GetString();
                serviceRequest.ServiceStatus = status;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Service request status updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("staff/service-requests/start/{id}")]
        public async Task<IActionResult> StartServiceRequest(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Json(new { success = false, message = "Not authorized" });
                }

                var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.LoginId == userId);
                if (staff == null)
                {
                    return Json(new { success = false, message = "Staff profile not found" });
                }

                var serviceRequest = await _context.ServiceRequests
                    .FirstOrDefaultAsync(sr => sr.ServiceRequestId == id && sr.StaffId == staff.StaffId);

                if (serviceRequest == null)
                {
                    return Json(new { success = false, message = "Service request not found or not assigned to you" });
                }

                if (serviceRequest.ServiceStatus != "Pending" && serviceRequest.ServiceStatus != "Assigned")
                {
                    return Json(new { success = false, message = "Service request cannot be started" });
                }

                serviceRequest.ServiceStatus = "In Progress";
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Service request started successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("staff/service-requests/end/{id}")]
        public async Task<IActionResult> EndServiceRequest(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Json(new { success = false, message = "Not authorized" });
                }

                var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.LoginId == userId);
                if (staff == null)
                {
                    return Json(new { success = false, message = "Staff profile not found" });
                }

                var serviceRequest = await _context.ServiceRequests
                    .FirstOrDefaultAsync(sr => sr.ServiceRequestId == id && sr.StaffId == staff.StaffId);

                if (serviceRequest == null)
                {
                    return Json(new { success = false, message = "Service request not found or not assigned to you" });
                }

                if (serviceRequest.ServiceStatus != "In Progress")
                {
                    return Json(new { success = false, message = "Only in-progress service requests can be completed" });
                }

                serviceRequest.ServiceStatus = "Completed";
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Service request completed successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("staff/service-requests/complete/{id}")]
        public async Task<IActionResult> CompleteServiceRequest(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null)
            {
                return NotFound("Service request not found.");
            }

            try
            {
                serviceRequest.ServiceStatus = "Completed";
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Service request completed successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("staff/events/list")]
        public async Task<IActionResult> GetEvents([FromQuery] DateTime? start = null, [FromQuery] DateTime? end = null)
        {
            try
            {
                if (!start.HasValue)
                {
                    start = new DateTime(DateTime.Now.Year, 1, 1);
                }
                if (!end.HasValue)
                {
                    end = new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 59);
                }

                var events = await _context.EventCalendars
                    .Include(e => e.OrganizedBy)
                        .ThenInclude(a => a.User)
                    .Where(e => e.EventDateTime >= start && e.EventDateTime <= end)
                    .Select(e => new
                    {
                        eventId = e.EventId,
                        eventTitle = e.EventTitle,
                        eventDesc = e.EventDesc,
                        eventDateTime = e.EventDateTime,
                        visibility = e.Visibility,
                        organizedBy = e.OrganizedBy != null && e.OrganizedBy.User != null 
                            ? $"{e.OrganizedBy.User.FirstName} {e.OrganizedBy.User.LastName}"
                            : "Admin"
                    })
                    .ToListAsync();

                return Ok(events);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to retrieve events: {ex.Message}" });
            }
        }

        public IActionResult Events()
        {
            if (!IsLoggedIn() || !IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }
            ViewData["Page"] = "Events";
            ViewData["UserType"] = "Staff";
            return View();
        }

        public IActionResult Community()
        {
            if (!IsLoggedIn() || !IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }
            ViewData["Page"] = "Community";
            ViewData["UserType"] = "Staff";

            // Fetch announcements and forums with their replies from the database
            var announcements = _context.Announcements
                .OrderByDescending(a => a.DateTimePosted)
                .ToList();

            var forums = _context.Forums
                .Include(f => f.ForumReplies)
                    .ThenInclude(r => r.Admin)
                .Include(f => f.ForumReplies)
                    .ThenInclude(r => r.Staff)
                .Include(f => f.ForumReplies)
                    .ThenInclude(r => r.Homeowner)
                .OrderByDescending(f => f.DateTimePosted)
                .ToList();

            // Pass as tuple to the view (forums, announcements)
            var model = (forums, announcements);
            return View(model);
        }

        [HttpPost]
        [Route("staff/community/create-forum")]
        public async Task<IActionResult> CreateForum([FromForm] Forum forum, IFormFile? image)
        {
            try
            {
                if (string.IsNullOrEmpty(forum.ForumTitle) || string.IsNullOrEmpty(forum.Content))
                {
                    TempData["CommunityError"] = "Title and content are required";
                    return RedirectToAction("Community");
                }

                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    TempData["CommunityError"] = "Not authorized";
                    return RedirectToAction("Community");
                }

                var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.LoginId == userId);
                if (staff == null)
                {
                    TempData["CommunityError"] = "Staff profile not found.";
                    return RedirectToAction("Community");
                }

                if (image != null && image.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "forums");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    forum.ImagePath = "/uploads/forums/" + uniqueFileName;
                }

                forum.StaffId = staff.StaffId;
                forum.AdminId = null;
                forum.HomeownerId = null;
                forum.DateTimePosted = DateTime.Now;
                forum.ForumReplies = new List<ForumReplies>();

                _context.Forums.Add(forum);
                await _context.SaveChangesAsync();

                TempData["CommunitySuccess"] = "Forum post created successfully";
            }
            catch (Exception ex)
            {
                TempData["CommunityError"] = "Error creating forum post: " + ex.Message;
            }
            return RedirectToAction("Community");
        }

        [HttpPost]
        [Route("staff/community/add-contact")]
        public async Task<IActionResult> AddContact([Bind("ContactPersonName,PhoneNumber,Email,Category")] Contact contact)
        {
            var staffId = HttpContext.Session.GetInt32("UserId");
            if (staffId == null)
            {
                TempData["CommunityError"] = "Not authorized.";
                return RedirectToAction("Community");
            }

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            TempData["CommunitySuccess"] = "Contact added successfully!";
            return RedirectToAction("Community");
        }

        [HttpPost]
        [Route("staff/community/add-reply")]
        public async Task<IActionResult> AddForumReply(int forumId, string content)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["CommunityError"] = "Not authorized";
                return RedirectToAction("Community");
            }

            try
            {
                var staff = await _context.Staffs.FirstOrDefaultAsync(s => s.LoginId == userId);
                if (staff == null)
                {
                    TempData["CommunityError"] = "Staff profile not found";
                    return RedirectToAction("Community");
                }

                var forum = await _context.Forums.FindAsync(forumId);
                if (forum == null)
                {
                    TempData["CommunityError"] = "Forum post not found";
                    return RedirectToAction("Community");
                }

                var reply = new ForumReplies
                {
                    ForumId = forumId,
                    AdminId = null,
                    StaffId = staff.StaffId,
                    HomeownerId = null,
                    RepliedContent = content,
                    DateTime = DateTime.Now
                };

                _context.ForumReplies.Add(reply);
                await _context.SaveChangesAsync();

                TempData["CommunitySuccess"] = "Reply added successfully";
            }
            catch (Exception ex)
            {
                TempData["CommunityError"] = "Error adding reply: " + ex.Message;
            }

            return RedirectToAction("Community");
        }

        [Route("staff/contacts")]
        public async Task<IActionResult> Contacts()
        {
            ViewData["UserType"] = "Staff";
            ViewData["Page"] = "Contacts";
            var contacts = await _context.Contacts.ToListAsync();
            return View("~/Views/Staff/Contacts.cshtml", contacts);
        }

        [Route("staff/servicerequest")]
        public IActionResult ServiceRequest()
        {
            if (!IsLoggedIn() || !IsStaff())
            {
                return RedirectToAction("Login", "Auth");
            }
            ViewData["Page"] = "ServiceRequest";
            ViewData["UserType"] = "Staff";
            return View();
        }
    }
}