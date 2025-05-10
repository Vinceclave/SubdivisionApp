using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Subdivision.Models;
using Subdivision.Data;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Subdivision.Controllers
{
    public class HomeownerController : Controller
    {
        private readonly SubdivisionDbContext _context;

        public HomeownerController(SubdivisionDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn() => HttpContext.Session.GetInt32("UserId") != null;
        private bool IsHomeowner() => HttpContext.Session.GetInt32("UserType") == (int)UserType.Homeowner;

        public IActionResult Index()
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["Page"] = "Dashboard";
            ViewData["UserType"] = "Homeowner";
            return View();
        }

        public IActionResult MyReservations()
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["Page"] = "MyReservations";
            ViewData["UserType"] = "Homeowner";
            return View();
        }

        public IActionResult ServiceRequests()
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["Page"] = "ServiceRequests";
            ViewData["UserType"] = "Homeowner";
            return View();
        }

        public IActionResult Payments()
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["Page"] = "Payments";
            ViewData["UserType"] = "Homeowner";
            return View();
        }

        public async Task<IActionResult> Profile()
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _context.Users
                .Include(u => u.Homeowner)
                    .ThenInclude(h => h.Forums.Where(f => !string.IsNullOrEmpty(f.ImagePath)))
                .FirstOrDefaultAsync(u => u.LoginId == userId.Value);

            if (user == null)
            {
                return NotFound();
            }

            ViewData["Page"] = "Profile";
            ViewData["UserType"] = "Homeowner";
            return View(user);
        }

        [HttpPost]
        [Route("homeowner/profile/update")]
        public async Task<IActionResult> UpdateProfile([FromForm] IFormFile? photo, [FromForm] string? removePhoto)
        {
            if (!IsLoggedIn() || !IsHomeowner())
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

        public async Task<IActionResult> Settings()
        {
            if (!IsLoggedIn() || !IsHomeowner())
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

            ViewData["Page"] = "Settings";
            ViewData["UserType"] = "Homeowner";
            return View(user);
        }

        [HttpPost]
        [Route("homeowner/settings/update")]
        public async Task<IActionResult> UpdateSettings([FromForm] User model, IFormFile? Photo, string newPassword, string confirmPassword)
        {
            if (!IsLoggedIn() || !IsHomeowner())
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

        public IActionResult FacilitiesReservation()
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["Page"] = "FacilitiesReservation";
            ViewData["UserType"] = "Homeowner";
            return View();
        }

        [HttpGet]
        [Route("homeowner/community")]
        public IActionResult Community()
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["Page"] = "Community";
            ViewData["UserType"] = "Homeowner";

            // Fetch announcements and forums with replies from the database
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

            // Create a tuple to pass to the view
            var model = (forums, announcements);

            return View(model);
        }

        [HttpGet]
        [Route("homeowner/community/announcements")]
        public IActionResult GetAnnouncements()
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return Json(new { success = false, message = "Unauthorized access" });
            }

            try
            {
                var announcements = _context.Announcements
                    .OrderByDescending(a => a.DateTimePosted)
                    .Take(5) // Only get the 5 most recent announcements
                    .Select(a => new
                    {
                        a.AnnouncementId,
                        a.Title,
                        a.Description,
                        a.DateTimePosted
                    })
                    .ToList();

                return Json(new { success = true, announcements });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpGet]
        [Route("homeowner/community/forums")]
        public IActionResult GetForums()
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return Json(new { success = false, message = "Unauthorized access" });
            }

            try
            {
                var forums = _context.Forums
                    .Include(f => f.ForumReplies)
                        .ThenInclude(r => r.Admin)
                    .Include(f => f.ForumReplies)
                        .ThenInclude(r => r.Staff)
                    .Include(f => f.ForumReplies)
                        .ThenInclude(r => r.Homeowner)
                    .OrderByDescending(f => f.DateTimePosted)
                    .Select(f => new
                    {
                        f.ForumId,
                        f.ForumTitle,
                        f.Content,
                        f.DateTimePosted,
                        f.ImagePath,
                        f.HomeownerId,
                        f.AdminId,
                        f.StaffId,
                        replies = f.ForumReplies.Select(r => new {
                            r.ForumRepliesId,  // Changed from ForumReplyId to ForumRepliesId
                            r.RepliedContent,
                            r.DateTime,
                            r.HomeownerId,
                            r.AdminId,
                            r.StaffId
                        }).OrderBy(r => r.DateTime).ToList(),
                        posterInfo = new { 
                            isHomeowner = f.HomeownerId.HasValue,
                            isAdmin = f.AdminId.HasValue,
                            isStaff = f.StaffId.HasValue
                        }
                    })
                    .ToList();

                return Json(new { success = true, forums });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("homeowner/community/create-forum")]
        public async Task<IActionResult> CreateForum([FromForm] string ForumTitle, [FromForm] string Content, [FromForm] IFormFile image)
        {
            if (!IsLoggedIn() || !IsHomeowner())
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

                var user = await _context.Users
                    .Include(u => u.Homeowner)
                    .FirstOrDefaultAsync(u => u.LoginId == userId.Value);

                if (user?.Homeowner == null)
                {
                    return Json(new { success = false, message = "Homeowner profile not found" });
                }

                // Create the forum post
                var forum = new Forum
                {
                    ForumTitle = ForumTitle,
                    Content = Content,
                    HomeownerId = user.Homeowner.HomeownerId,
                    AdminId = null,
                    StaffId = null,
                    DateTimePosted = DateTime.Now
                };

                // Handle image upload if provided
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

                _context.Forums.Add(forum);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Forum post created successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("homeowner/community/add-reply")]
        public async Task<IActionResult> AddForumReply(int forumId, string content)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Not authorized" });
            }

            try
            {
                var homeowner = await _context.Homeowners.FirstOrDefaultAsync(h => h.LoginId == userId);
                if (homeowner == null)
                {
                    return Json(new { success = false, message = "Homeowner profile not found" });
                }

                var forum = await _context.Forums.FindAsync(forumId);
                if (forum == null)
                {
                    return Json(new { success = false, message = "Forum post not found" });
                }

                var reply = new ForumReplies
                {
                    ForumId = forumId,
                    AdminId = null,
                    StaffId = null,
                    HomeownerId = homeowner.HomeownerId,
                    RepliedContent = content,
                    DateTime = DateTime.Now
                };

                _context.ForumReplies.Add(reply);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Reply added successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding reply: " + ex.Message });
            }
        }

        [HttpGet]
        [Route("homeowner/facilities")]
        public async Task<IActionResult> GetFacilities()
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return Json(new { success = false, message = "Unauthorized access" });
            }

            try
            {
                var facilities = await _context.Facilities.Select(f => new
                {
                    f.FacilityId,
                    f.FacilityName,
                    f.TypeOfFacility,
                    f.FacilityCapacity,
                    f.FacilityImg,
                    f.WorkingDays,
                    f.WorkingHours,
                    f.Status,
                    f.RulesRegulations
                }).ToListAsync();

                return Json(new { success = true, facilities });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("homeowner/reservations/create")]
        public async Task<IActionResult> CreateReservation([FromBody] Reservation model)
        {
            if (!IsLoggedIn() || !IsHomeowner())
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

                var user = await _context.Users
                    .Include(u => u.Homeowner)
                    .FirstOrDefaultAsync(u => u.LoginId == userId.Value);

                if (user?.Homeowner == null)
                {
                    return Json(new { success = false, message = "Homeowner profile not found" });
                }

                // Check if the facility exists
                var facility = await _context.Facilities.FindAsync(model.FacilityId);
                if (facility == null)
                {
                    return Json(new { success = false, message = "Facility not found" });
                }

                // Check if the facility is available
                if (facility.Status.ToLower() != "available")
                {
                    return Json(new { success = false, message = "This facility is not available for reservation" });
                }

                // Check if there's a conflicting reservation
                var conflictingReservation = await _context.Reservations
                    .Where(r => r.FacilityId == model.FacilityId)
                    .Where(r => r.DateOfReservation.Date == model.DateOfReservation.Date)
                    .Where(r => r.Status != "Rejected" && r.Status != "Cancelled")
                    .Where(r => (model.ReservationTimeIn >= r.ReservationTimeIn && model.ReservationTimeIn < r.ReservationTimeOut) ||
                                (model.ReservationTimeOut > r.ReservationTimeIn && model.ReservationTimeOut <= r.ReservationTimeOut) ||
                                (model.ReservationTimeIn <= r.ReservationTimeIn && model.ReservationTimeOut >= r.ReservationTimeOut))
                    .FirstOrDefaultAsync();

                if (conflictingReservation != null)
                {
                    return Json(new { success = false, message = "This time slot is already reserved" });
                }

                // Create the reservation
                var reservation = new Reservation
                {
                    FacilityId = model.FacilityId,
                    HomeownerId = user.Homeowner.HomeownerId,
                    AdminId = 1, // Using default admin ID (usually the first admin in the system)
                    Capacity = model.Capacity,
                    ReservationTimeIn = model.ReservationTimeIn,
                    ReservationTimeOut = model.ReservationTimeOut,
                    DateOfReservation = model.DateOfReservation,
                    Status = "Approved" // Changed from "Pending" to "Approved"
                };

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Reservation created successfully", reservationId = reservation.ReservationId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpGet]
        [Route("homeowner/reservations")]
        public async Task<IActionResult> GetMyReservations()
        {
            if (!IsLoggedIn() || !IsHomeowner())
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

                var user = await _context.Users
                    .Include(u => u.Homeowner)
                    .FirstOrDefaultAsync(u => u.LoginId == userId.Value);

                if (user?.Homeowner == null)
                {
                    return Json(new { success = false, message = "Homeowner profile not found" });
                }

                var reservations = await _context.Reservations
                    .Where(r => r.HomeownerId == user.Homeowner.HomeownerId)
                    .Include(r => r.Facility)
                    .OrderByDescending(r => r.DateOfReservation)
                    .Select(r => new
                    {
                        r.ReservationId,
                        FacilityName = r.Facility.FacilityName,
                        FacilityType = r.Facility.TypeOfFacility,
                        FacilityImage = r.Facility.FacilityImg,
                        r.DateOfReservation,
                        TimeIn = r.ReservationTimeIn.ToString("h:mm tt"),
                        TimeOut = r.ReservationTimeOut.ToString("h:mm tt"),
                        r.Capacity,
                        r.Status
                    })
                    .ToListAsync();

                return Json(new { success = true, reservations });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("homeowner/reservations/{id}/cancel")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            if (!IsLoggedIn() || !IsHomeowner())
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

                var user = await _context.Users
                    .Include(u => u.Homeowner)
                    .FirstOrDefaultAsync(u => u.LoginId == userId.Value);

                if (user?.Homeowner == null)
                {
                    return Json(new { success = false, message = "Homeowner profile not found" });
                }

                var reservation = await _context.Reservations
                    .FirstOrDefaultAsync(r => r.ReservationId == id && r.HomeownerId == user.Homeowner.HomeownerId);

                if (reservation == null)
                {
                    return Json(new { success = false, message = "Reservation not found" });
                }

                if (reservation.Status != "Pending" && reservation.Status != "Approved")
                {
                    return Json(new { success = false, message = "Only pending or approved reservations can be cancelled" });
                }

                reservation.Status = "Cancelled";
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Reservation cancelled successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        public async Task<IActionResult> Contacts()
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["Page"] = "Contacts";
            ViewData["UserType"] = "Homeowner";
            
            var contacts = await _context.Contacts.ToListAsync();
            return View(contacts);
        }

        [HttpGet]
        [Route("homeowner/service-requests/list")]
        public async Task<IActionResult> GetServiceRequests()
        {
            if (!IsLoggedIn() || !IsHomeowner())
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

                var user = await _context.Users
                    .Include(u => u.Homeowner)
                    .FirstOrDefaultAsync(u => u.LoginId == userId.Value);

                if (user?.Homeowner == null)
                {
                    return Json(new { success = false, message = "Homeowner profile not found" });
                }

                var requests = await _context.ServiceRequests
                    .Where(sr => sr.HomeownerId == user.Homeowner.HomeownerId)
                    .Include(sr => sr.Staff)
                    .OrderByDescending(sr => sr.RequestDateTime)
                    .Select(sr => new
                    {
                        sr.ServiceRequestId,
                        sr.RequestType,
                        sr.Priority,
                        sr.Description,
                        sr.RequestDateTime,
                        sr.ServiceStatus,
                        sr.StaffId,
                        StaffName = sr.Staff != null ? sr.Staff.User.FirstName + " " + sr.Staff.User.LastName : null
                    })
                    .ToListAsync();

                return Json(new { success = true, requests });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("homeowner/service-requests/create")]
        public async Task<IActionResult> CreateServiceRequest([FromBody] ServiceRequestViewModel model)
        {
            if (!IsLoggedIn() || !IsHomeowner())
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

                var user = await _context.Users
                    .Include(u => u.Homeowner)
                    .FirstOrDefaultAsync(u => u.LoginId == userId.Value);

                if (user?.Homeowner == null)
                {
                    return Json(new { success = false, message = "Homeowner profile not found" });
                }

                var serviceRequest = new ServiceRequest
                {
                    HomeownerId = user.Homeowner.HomeownerId,
                    RequestType = model.RequestType,
                    Priority = model.Priority,
                    Description = model.Description,
                    RequestDateTime = DateTime.Now,
                    ServiceStatus = "Pending"
                };

                _context.ServiceRequests.Add(serviceRequest);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Service request created successfully", requestId = serviceRequest.ServiceRequestId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("homeowner/service-requests/{id}/cancel")]
        public async Task<IActionResult> CancelServiceRequest(int id)
        {
            if (!IsLoggedIn() || !IsHomeowner())
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

                var user = await _context.Users
                    .Include(u => u.Homeowner)
                    .FirstOrDefaultAsync(u => u.LoginId == userId.Value);

                if (user?.Homeowner == null)
                {
                    return Json(new { success = false, message = "Homeowner profile not found" });
                }

                var serviceRequest = await _context.ServiceRequests
                    .FirstOrDefaultAsync(sr => sr.ServiceRequestId == id && sr.HomeownerId == user.Homeowner.HomeownerId);

                if (serviceRequest == null)
                {
                    return Json(new { success = false, message = "Service request not found" });
                }

                if (serviceRequest.ServiceStatus != "Pending")
                {
                    return Json(new { success = false, message = "Only pending service requests can be cancelled" });
                }

                serviceRequest.ServiceStatus = "Cancelled";
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Service request cancelled successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("homeowner/service-requests/start/{id}")]
        public async Task<IActionResult> StartServiceRequest(int id)
        {
            if (!IsLoggedIn() || !IsHomeowner())
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

                var user = await _context.Users
                    .Include(u => u.Homeowner)
                    .FirstOrDefaultAsync(u => u.LoginId == userId.Value);

                if (user?.Homeowner == null)
                {
                    return Json(new { success = false, message = "Homeowner profile not found" });
                }

                var serviceRequest = await _context.ServiceRequests
                    .FirstOrDefaultAsync(sr => sr.ServiceRequestId == id && sr.HomeownerId == user.Homeowner.HomeownerId);

                if (serviceRequest == null)
                {
                    return Json(new { success = false, message = "Service request not found" });
                }

                if (serviceRequest.ServiceStatus != "Pending")
                {
                    return Json(new { success = false, message = "Only pending service requests can be started" });
                }

                serviceRequest.ServiceStatus = "In Progress";
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Service request marked as ongoing" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("homeowner/service-requests/end/{id}")]
        public async Task<IActionResult> EndServiceRequest(int id)
        {
            if (!IsLoggedIn() || !IsHomeowner())
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

                var user = await _context.Users
                    .Include(u => u.Homeowner)
                    .FirstOrDefaultAsync(u => u.LoginId == userId.Value);

                if (user?.Homeowner == null)
                {
                    return Json(new { success = false, message = "Homeowner profile not found" });
                }

                var serviceRequest = await _context.ServiceRequests
                    .FirstOrDefaultAsync(sr => sr.ServiceRequestId == id && sr.HomeownerId == user.Homeowner.HomeownerId);

                if (serviceRequest == null)
                {
                    return Json(new { success = false, message = "Service request not found" });
                }

                if (serviceRequest.ServiceStatus != "In Progress")
                {
                    return Json(new { success = false, message = "Only ongoing service requests can be completed" });
                }

                serviceRequest.ServiceStatus = "Completed";
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Service request marked as completed" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpGet]
        [Route("homeowner/events/list")]
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
                    .Where(e => e.EventDateTime >= start && e.EventDateTime <= end && e.Visibility.ToLower() == "public")
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

        [HttpGet]
        [Route("api/homeowner/feedback")]
        public async Task<IActionResult> GetFeedback()
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return Unauthorized();
            }

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var feedback = await _context.Feedbacks
                    .Where(f => f.Homeowner.User.LoginId == userId)
                    .OrderByDescending(f => f.FeedbackDate)
                    .Select(f => new
                    {
                        f.FeedbackId,
                        f.Rating,
                        f.FeedbackContent,
                        f.FeedbackDate
                    })
                    .ToListAsync();

                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving feedback" });
            }
        }

        [HttpPost]
        [Route("api/homeowner/feedback")]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackViewModel model)
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return Unauthorized();
            }

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var user = await _context.Users
                    .Include(u => u.Homeowner)
                    .FirstOrDefaultAsync(u => u.LoginId == userId);

                if (user?.Homeowner == null)
                {
                    return NotFound(new { message = "Homeowner profile not found" });
                }

                var feedback = new Feedback
                {
                    HomeownerId = user.Homeowner.HomeownerId,
                    Rating = model.Rating,
                    FeedbackContent = model.FeedbackContent,
                    FeedbackDate = DateTime.UtcNow
                };

                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Feedback submitted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while submitting feedback" });
            }
        }

        [HttpGet]
        [Route("api/homeowner/complaints")]
        public async Task<IActionResult> GetComplaints()
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return Unauthorized();
            }

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var complaints = await _context.Complaints
                    .Where(c => c.Homeowner.User.LoginId == userId)
                    .OrderByDescending(c => c.ComplaintDate)
                    .Select(c => new
                    {
                        c.ComplaintId,
                        c.ComplaintContent,
                        c.ComplaintDate,
                        c.Status
                    })
                    .ToListAsync();

                return Ok(complaints);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving complaints" });
            }
        }

        [HttpPost]
        [Route("api/homeowner/complaints")]
        public async Task<IActionResult> CreateComplaint([FromBody] ComplaintViewModel model)
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return Unauthorized();
            }

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var user = await _context.Users
                    .Include(u => u.Homeowner)
                    .FirstOrDefaultAsync(u => u.LoginId == userId);

                if (user?.Homeowner == null)
                {
                    return NotFound(new { message = "Homeowner profile not found" });
                }

                var complaint = new Complaint
                {
                    HomeownerId = user.Homeowner.HomeownerId,
                    ComplaintContent = model.ComplaintContent,
                    ComplaintDate = DateTime.UtcNow,
                    Status = "Pending"
                };

                _context.Complaints.Add(complaint);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Complaint submitted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while submitting complaint" });
            }
        }

        public IActionResult FeedbackAndComplaints()
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["Page"] = "FeedbackAndComplaints";
            ViewData["UserType"] = "Homeowner";
            return View();
        }

        [HttpGet]
        [Route("homeowner/payments/bills")]
        public async Task<IActionResult> GetBills()
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return Unauthorized(new { message = "Unauthorized access" });
            }

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var user = await _context.Users
                    .Include(u => u.Homeowner)
                    .FirstOrDefaultAsync(u => u.LoginId == userId);

                if (user?.Homeowner == null)
                {
                    return NotFound(new { message = "Homeowner profile not found" });
                }

                var bills = await _context.Bills
                    .Include(b => b.Payments)
                    .Where(b => b.HomeownerId == user.Homeowner.HomeownerId)
                    .OrderByDescending(b => b.DueDate)
                    .Select(b => new
                    {
                        b.BillId,
                        b.BillType,
                        b.Amount,
                        b.DueDate,
                        b.Status,
                        AmountPaid = b.Payments.Sum(p => p.AmountPaid),
                        Payments = b.Payments.Select(p => new
                        {
                            p.PaymentId,
                            p.AmountPaid,
                            p.ModeOfPayment,
                            p.PaymentDate
                        })
                    })
                    .ToListAsync();

                return Ok(new { success = true, bills });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("homeowner/payments/pay")]
        public async Task<IActionResult> MakePayment([FromBody] PaymentViewModel model)
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return Unauthorized(new { message = "Unauthorized access" });
            }

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var user = await _context.Users
                    .Include(u => u.Homeowner)
                    .FirstOrDefaultAsync(u => u.LoginId == userId);

                if (user?.Homeowner == null)
                {
                    return NotFound(new { message = "Homeowner profile not found" });
                }

                var bill = await _context.Bills
                    .Include(b => b.Payments)
                    .FirstOrDefaultAsync(b => b.BillId == model.BillId && b.HomeownerId == user.Homeowner.HomeownerId);

                if (bill == null)
                {
                    return NotFound(new { message = "Bill not found" });
                }

                var totalPaid = bill.Payments.Sum(p => p.AmountPaid) + model.AmountPaid;
                if (totalPaid > bill.Amount)
                {
                    return BadRequest(new { message = "Payment amount exceeds bill balance" });
                }

                var payment = new Payment
                {
                    BillId = model.BillId,
                    HomeownerId = user.Homeowner.HomeownerId,
                    AmountPaid = model.AmountPaid,
                    ModeOfPayment = model.ModeOfPayment,
                    PaymentDate = DateTime.Now,
                    Status = "Pending"
                };

                if (model.ModeOfPayment == "Credit/Debit Card" && model.CardDetails != null)
                {
                    var card = new CreditDebitCard
                    {
                        CardholderName = model.CardDetails.CardholderName,
                        CardNumber = model.CardDetails.CardNumber,
                        ExpiryDate = model.CardDetails.ExpiryDate,
                        CVV = model.CardDetails.CVV
                    };
                    _context.CreditDebitCards.Add(card);
                    await _context.SaveChangesAsync();
                    payment.CardId = card.CardId;
                }
                else if (model.ModeOfPayment == "Online Banking" && model.BankDetails != null)
                {
                    var bank = new OnlineBanking
                    {
                        BankName = model.BankDetails.BankName,
                        AccountHolderName = model.BankDetails.AccountHolderName,
                        AccountNumber = model.BankDetails.AccountNumber,
                        TransactionRefNumber = model.BankDetails.TransactionRefNumber
                    };
                    _context.OnlineBankings.Add(bank);
                    await _context.SaveChangesAsync();
                    payment.OnlineBankingId = bank.OnlineBankingId;
                }

                _context.Payments.Add(payment);
                
                // Update bill status if fully paid
                if (Math.Abs(totalPaid - bill.Amount) < 0.01m) // Using small epsilon for decimal comparison
                {
                    bill.Status = "Paid";
                }

                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Payment processed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        public IActionResult Events()
        {
            if (!IsLoggedIn() || !IsHomeowner())
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["Page"] = "Events";
            ViewData["UserType"] = "Homeowner";
            return View();
        }
    }

    // View Model for Service Request
    public class ServiceRequestViewModel
    {
        public string RequestType { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class FeedbackViewModel
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000)]
        public string FeedbackContent { get; set; } = string.Empty;
    }

    public class ComplaintViewModel
    {
        [Required]
        [StringLength(2000)]
        public string ComplaintContent { get; set; } = string.Empty;
    }

    public class PaymentViewModel
    {
        public int BillId { get; set; }
        public decimal AmountPaid { get; set; }
        public string ModeOfPayment { get; set; } = string.Empty;
        public CardDetails? CardDetails { get; set; }
        public BankDetails? BankDetails { get; set; }
    }

    public class CardDetails
    {
        public string CardholderName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public string CVV { get; set; } = string.Empty;
    }

    public class BankDetails
    {
        public string BankName { get; set; } = string.Empty;
        public string AccountHolderName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string TransactionRefNumber { get; set; } = string.Empty;
    }
}