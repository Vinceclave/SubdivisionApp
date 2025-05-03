using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Subdivision.Data;
using Subdivision.Models;
using Subdivision.Filters;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Subdivision.Controllers
{
    [AdminAuthorization]
    public class AdminController : Controller
    {
        private readonly SubdivisionDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(SubdivisionDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Dashboard";
            ViewData["UserType"] = "Admin";
            ViewData["Page"] = "Dashboard";

            // Get counts for the dashboard
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalResidents = await _context.Homeowners.CountAsync();
            ViewBag.TotalStaff = await _context.Staffs.CountAsync();

            // Returning view with layout
            return View("~/Views/Admin/Index.cshtml"); // This view will be rendered with the _Layout.cshtml
        }

        [Route("admin/dashboard")]
        public IActionResult Dashboard()
        {
            ViewData["UserType"] = "Admin";
            ViewData["Page"] = "Dashboard";
            return View("~/Views/Admin/Dashboard.cshtml");
        }

        [Route("admin/contacts")]
        public async Task<IActionResult> Contacts()
        {
            ViewData["UserType"] = "Admin";
            ViewData["Page"] = "Contacts";
            var contacts = await _context.Contacts.ToListAsync(); // Fetch the list of contacts
            return View("~/Views/Admin/Contacts.cshtml", contacts); // Pass the list to the view
        }

        [Route("admin/community")]
        public IActionResult Community()
        {
            ViewData["UserType"] = "Admin";
            ViewData["Page"] = "Community";

            // Fetch announcements and forums from the database
            var announcements = _context.Announcements.ToList(); // Fetch announcements
            var forums = _context.Forums.ToList(); // Fetch forums

            // Create a ValueTuple to pass to the view
            var model = (forums, announcements); // This creates a tuple of (List<Forum>, List<Announcement>)

            return View("~/Views/Admin/Community.cshtml", model); // Pass the ValueTuple to the view
        }

        [Route("admin/events")]
        public IActionResult Events()
        {
            ViewData["UserType"] = "Admin";
            ViewData["Page"] = "Events";
            return View("~/Views/Admin/Events.cshtml");
        }

        [Route("admin/reservations")]
        public async Task<IActionResult> Reservations()
        {
            ViewData["UserType"] = "Admin";
            ViewData["Page"] = "Reservations";
            
            // Get facilities and pending reservations for the view
            var facilities = await _context.Facilities
                .Include(f => f.Admin)
                    .ThenInclude(a => a.User)
                .ToListAsync();
                
            var pendingReservations = await _context.Reservations
                .Where(r => r.Status == "Pending")
                .Include(r => r.Facility)
                .Include(r => r.Homeowner)
                    .ThenInclude(h => h.User)
                .ToListAsync();
            
            // Create a model for the view
            var model = new
            {
                Facilities = facilities,
                PendingReservations = pendingReservations
            };
            
            return View("~/Views/Admin/Reservations.cshtml", model);
        }

        [Route("admin/usermanagement")]
        public async Task<IActionResult> UserManagement()
        {
            ViewData["UserType"] = "Admin";
            ViewData["Page"] = "UserManagement";
            try
            {
                // Fetch users excluding those with UserType Admin
                var users = await _context.Users
                    .Where(u => u.UserType != UserType.Admin) // Exclude Admin users
                    .Include(u => u.Staff) // Include related Staff data if needed
                    .Include(u => u.Homeowner) // Include related Homeowner data if needed
                    .ToListAsync();

                return View("~/Views/Admin/UserManagement.cshtml", users); // Pass the filtered list of users to the view
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                TempData["Error"] = "An error occurred while retrieving users: " + ex.Message;
                return View("~/Views/Admin/UserManagement.cshtml", new List<User>()); // Return an empty list on error
            }
        }

        [Route("admin/eventcalendar")]
        public IActionResult EventCalendar()
        {
            ViewData["UserType"] = "Admin";
            ViewData["Page"] = "EventCalendar";
            return View("~/Views/Admin/EventCalendar.cshtml");
        }

        [Route("admin/files")]
        public IActionResult Files()
        {
            ViewData["UserType"] = "Admin";
            ViewData["Page"] = "Files";
            return View("~/Views/Admin/Files.cshtml");
        }

        [Route("admin/servicerequest")]
        public async Task<IActionResult> ServiceRequest()
        {
            ViewData["UserType"] = "Admin";
            ViewData["Page"] = "ServiceRequest";

            // Fetch service requests with all required navigation properties
            var serviceRequests = await _context.ServiceRequests
                .Include(sr => sr.Homeowner)
                    .ThenInclude(h => h.User)
                .Include(sr => sr.Staff)
                    .ThenInclude(s => s.User)
                .ToListAsync();

            return View("~/Views/Admin/ServiceRequest.cshtml", serviceRequests);
        }

        [Route("admin/feedback")]
        public IActionResult Feedback()
        {
            ViewData["UserType"] = "Admin";
            ViewData["Page"] = "Feedback";
            return View("~/Views/Admin/Feedback.cshtml");
        }

        [Route("admin/pollssurveys")]
        public IActionResult PollsSurveys()
        {
            ViewData["UserType"] = "Admin";
            ViewData["Page"] = "PollsSurveys";
            return View("~/Views/Admin/PollsSurveys.cshtml");
        }

        [Route("admin/settings")]
        public async Task<IActionResult> Settings()
        {
            ViewData["UserType"] = "Admin";
            ViewData["Page"] = "Settings";
            
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users.FindAsync(adminId.Value);
            if (user == null)
            {
                return NotFound(); // Return 404 if user not found
            }

            return View("~/Views/Admin/Settings.cshtml", user); // Pass the user model to the view
        }

        [Route("admin/logout")]
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

        [HttpPost]
        [Route("admin/community/create-announcement")]
        public async Task<IActionResult> CreateAnnouncement([Bind("Title,Description")] Announcement announcement)
        {
            if (!ModelState.IsValid)
            {
                TempData["CommunityError"] = "Please fill in all required fields";
                return RedirectToAction(nameof(Community));
            }

            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                TempData["Error"] = "Not authorized";
                return RedirectToAction(nameof(Community));
            }

            announcement.AdminId = adminId.Value;
            announcement.DateTimePosted = DateTime.Now;

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();
            
            TempData["CommunitySuccess"] = "Announcement created successfully";
            return RedirectToAction(nameof(Community));
        }

        [HttpPost]
        [Route("admin/community/create-forum")]
        public async Task<IActionResult> CreateForum([FromForm] Forum forum, IFormFile? image)
        {
            try
            {
                if (string.IsNullOrEmpty(forum.ForumTitle) || string.IsNullOrEmpty(forum.Content))
                {
                    TempData["Error"] = "Title and content are required";
                    return RedirectToAction(nameof(Community));
                }

                var adminId = HttpContext.Session.GetInt32("AdminId");
                if (adminId == null)
                {
                    TempData["Error"] = "Not authorized";
                    return RedirectToAction(nameof(Community));
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

                forum.AdminId = adminId.Value;
                forum.HomeownerId = null;
                forum.StaffId = null;
                forum.DateTimePosted = DateTime.Now;
                forum.ForumReplies = new List<ForumReplies>();
                
                _context.Forums.Add(forum);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Forum post created successfully";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error creating forum post: " + ex.Message;
            }
            
            return RedirectToAction(nameof(Community));
        }

        [HttpPost]
        [Route("admin/contacts/add")]
        public async Task<IActionResult> AddContact([FromForm] Contact contact)
        {
            try
            {
                var adminId = HttpContext.Session.GetInt32("AdminId");
                if (adminId == null)
                {
                    TempData["Error"] = "Not authorized. Please log in as an admin.";
                    return RedirectToAction(nameof(Contacts));
                }

                // Set the AdminId property
                contact.AdminId = adminId.Value;

                if (!ModelState.IsValid)
                {
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    TempData["Error"] = $"Please fill in all required fields correctly. {errors}";
                    return RedirectToAction(nameof(Contacts));
                }

                // Validate category
                if (!Enum.IsDefined(typeof(ContactCategory), contact.Category))
                {
                    TempData["Error"] = "Please select a valid category";
                    return RedirectToAction(nameof(Contacts));
                }

                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Contact added successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding contact");
                TempData["Error"] = $"An error occurred while adding the contact: {ex.Message}";
            }

            return RedirectToAction(nameof(Contacts));
        }

        [HttpGet]
        [Route("admin/usermanagement/edit/{id}")]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Staff)
                .Include(u => u.Homeowner)
                .FirstOrDefaultAsync(u => u.LoginId == id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(UserManagement));
            }

            return Json(new {
                loginId = user.LoginId,
                username = user.Username,
                firstName = user.FirstName,
                lastName = user.LastName,
                address = user.Address,
                phoneNumber = user.PhoneNumber,
                email = user.Email,
                userType = user.UserType.ToString(), // Ensure userType is returned as string
                staffPosition = user.Staff != null ? ((int)user.Staff.Position).ToString() : null // Return position as numeric value
            }); // Return user data as JSON
        }

        [HttpPost]
        [Route("admin/usermanagement/edit")]
        public async Task<IActionResult> EditUser([FromBody] dynamic userData)
        {
            try {
                // Extract data from dynamic object
                int loginId = (int)userData.LoginId;
                string firstName = (string)userData.FirstName;
                string lastName = (string)userData.LastName;
                string address = (string)userData.Address;
                string phoneNumber = (string)userData.PhoneNumber;
                
                // Handle StaffPosition, which could be a number or null
                int? staffPosition = null;
                if (userData.StaffPosition != null)
                {
                    staffPosition = int.Parse(userData.StaffPosition.ToString());
                }

                // Find the existing user in the database
                var existingUser = await _context.Users
                    .Include(u => u.Staff)
                    .FirstOrDefaultAsync(u => u.LoginId == loginId);
                
                if (existingUser == null)
                {
                    return NotFound("User not found."); // Return a not found response
                }

                // Update user fields
                existingUser.FirstName = firstName;
                existingUser.LastName = lastName;
                existingUser.Address = address;
                existingUser.PhoneNumber = phoneNumber;

                // Update staff position if user is staff and position was provided
                if (existingUser.UserType == UserType.Staff && existingUser.Staff != null && staffPosition.HasValue)
                {
                    // Make sure the position value is valid for the enum
                    if (Enum.IsDefined(typeof(Models.StaffPosition), staffPosition.Value))
                    {
                        existingUser.Staff.Position = (Models.StaffPosition)staffPosition.Value;
                    }
                }

                // Save changes to the database
                await _context.SaveChangesAsync();
                return Ok("User updated successfully."); // Return a success response
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error updating user: " + ex.Message); // Return a server error response
            }
        }

        [HttpPost]
        [Route("admin/usermanagement/create")]
        public async Task<IActionResult> CreateUser([Bind("Username,Password,FirstName,LastName,Address,PhoneNumber,Email,UserType")] User user, string StaffPosition)
        {
            if (!ModelState.IsValid)
            {
                TempData["UserManagementError"] = "Please fill in all required fields.";
                return RedirectToAction(nameof(UserManagement));
            }

            // Hash the password before saving
            user.Password = HashPassword(user.Password);

            // Add the user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Check the UserType and create corresponding Staff or Homeowner
            if (user.UserType == UserType.Staff)
            {
                // Default to Security (0)
                Models.StaffPosition staffPosition = Models.StaffPosition.Security;
                
                // Try to parse the position from the form as an integer
                if (!string.IsNullOrEmpty(StaffPosition) && int.TryParse(StaffPosition, out int positionValue))
                {
                    // Make sure the value is valid for the enum
                    if (Enum.IsDefined(typeof(Models.StaffPosition), positionValue))
                    {
                        staffPosition = (Models.StaffPosition)positionValue;
                    }
                }
                
                var staff = new Staff
                {
                    LoginId = user.LoginId, // Link to the User
                    Position = staffPosition
                };
                _context.Staffs.Add(staff);
            }
            else if (user.UserType == UserType.Homeowner)
            {
                var homeowner = new Homeowner
                {
                    LoginId = user.LoginId // Link to the User
                };
                _context.Homeowners.Add(homeowner);
            }

            await _context.SaveChangesAsync();
            TempData["UserManagementSuccess"] = "User created successfully.";
            return RedirectToAction(nameof(UserManagement));
        }

        [HttpGet]
        [Route("admin/usermanagement/view/{id}")]
        public async Task<IActionResult> ViewUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Staff)
                .Include(u => u.Homeowner)
                .FirstOrDefaultAsync(u => u.LoginId == id);
            
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(UserManagement));
            }

            return View("~/Views/Admin/ViewUser.cshtml", user); // Pass the user model to the view
        }

        [HttpPost]
        [Route("admin/usermanagement/delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(UserManagement));
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            TempData["Success"] = "User deleted successfully.";
            return RedirectToAction(nameof(UserManagement));
        }

        [Route("admin/profile")]
        public IActionResult Profile()
        {
            ViewData["UserType"] = "Admin";
            ViewData["Page"] = "Profile";
            
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = _context.Users
                .Include(u => u.Admin)
                    .ThenInclude(a => a.Forums.Where(f => !string.IsNullOrEmpty(f.ImagePath)))
                .FirstOrDefault(u => u.LoginId == adminId.Value);

            if (user == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Profile.cshtml", user);
        }

        [HttpPost]
        [Route("admin/profile/update")]
        public async Task<IActionResult> UpdateProfile([FromForm] IFormFile? photo, [FromForm] string? removePhoto)
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users.FindAsync(adminId.Value);
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

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        [HttpPost]
        [Route("admin/settings/update")]
        public async Task<IActionResult> UpdateUser([Bind("LoginId,FirstName,LastName,Address,PhoneNumber,Email")] User user, string newPassword, string confirmPassword)
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var existingUser = await _context.Users
                .Include(u => u.Admin)
                .FirstOrDefaultAsync(u => u.LoginId == user.LoginId);

            if (existingUser == null || existingUser.Admin == null || existingUser.Admin.AdminId != adminId)
            {
                TempData["Error"] = "Unauthorized access.";
                return RedirectToAction(nameof(Settings));
            }

            // Update user properties
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Address = user.Address;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Email = user.Email;

            // Handle password update
            if (!string.IsNullOrEmpty(newPassword))
            {
                if (newPassword != confirmPassword)
                {
                    TempData["Error"] = "Passwords do not match.";
                    return RedirectToAction(nameof(Settings));
                }
                existingUser.Password = HashPassword(newPassword);
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Settings updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error updating settings: " + ex.Message;
            }

            return RedirectToAction(nameof(Settings));
        }

        [HttpGet]
        [Route("admin/events/list")]
        public async Task<IActionResult> GetEvents([FromQuery] DateTime? start = null, [FromQuery] DateTime? end = null)
        {
            try
            {
                // If no dates provided, default to current year
                if (!start.HasValue)
                {
                    start = new DateTime(DateTime.Now.Year, 1, 1);
                }
                if (!end.HasValue)
                {
                    end = new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 59);
                }

                _logger.LogInformation($"Fetching events from {start} to {end}");

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

                _logger.LogInformation($"Found {events.Count} events");

                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching events");
                return BadRequest(new { message = $"Failed to retrieve events: {ex.Message}" });
            }
        }

        [HttpPost]
        [Route("admin/events/create")]
        public async Task<IActionResult> CreateEvent([FromBody] EventCalendar eventData)
        {
            try
            {
                var adminId = HttpContext.Session.GetInt32("AdminId");
                if (adminId == null)
                {
                    return Unauthorized(new { message = "Not authorized" });
                }

                var newEvent = new EventCalendar
                {
                    EventTitle = eventData.EventTitle,
                    EventDesc = eventData.EventDesc,
                    EventDateTime = eventData.EventDateTime,
                    Visibility = eventData.Visibility ?? "public",
                    OrganizedById = adminId.Value,
                    DateUploaded = DateTime.UtcNow
                };

                _context.EventCalendars.Add(newEvent);
                await _context.SaveChangesAsync();

                var organizer = await _context.Admins
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.AdminId == adminId.Value);

                var responseEvent = new
                {
                    eventId = newEvent.EventId,
                    eventTitle = newEvent.EventTitle,
                    eventDesc = newEvent.EventDesc,
                    eventDateTime = newEvent.EventDateTime,
                    visibility = newEvent.Visibility,
                    dateUploaded = newEvent.DateUploaded,
                    organizedBy = organizer?.User != null 
                        ? $"{organizer.User.FirstName} {organizer.User.LastName}"
                        : "Admin"
                };

                return Ok(responseEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                return BadRequest(new { message = $"Failed to create event: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("admin/events/{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            try
            {
                var evt = await _context.EventCalendars
                    .Include(e => e.OrganizedBy)
                        .ThenInclude(a => a.User)
                    .FirstOrDefaultAsync(e => e.EventId == id);

                if (evt == null)
                {
                    return NotFound(new { message = "Event not found" });
                }

                var eventDetails = new
                {
                    eventId = evt.EventId,
                    eventTitle = evt.EventTitle,
                    eventDesc = evt.EventDesc,
                    eventDateTime = evt.EventDateTime,
                    visibility = evt.Visibility,
                    dateUploaded = evt.DateUploaded,
                    organizedBy = evt.OrganizedBy?.User != null 
                        ? $"{evt.OrganizedBy.User.FirstName} {evt.OrganizedBy.User.LastName}"
                        : "Admin"
                };

                return Ok(eventDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving event details");
                return BadRequest(new { message = $"Failed to retrieve event details: {ex.Message}" });
            }
        }

        [HttpDelete]
        [Route("admin/events/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var eventToDelete = await _context.EventCalendars.FindAsync(id);
                if (eventToDelete == null)
                {
                    return NotFound(new { message = "Event not found" });
                }

                var adminId = HttpContext.Session.GetInt32("AdminId");
                if (adminId == null || eventToDelete.OrganizedById != adminId)
                {
                    return Unauthorized(new { message = "Not authorized to delete this event" });
                }

                _context.EventCalendars.Remove(eventToDelete);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Event deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to delete event: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("admin/facilities")]
        public async Task<IActionResult> GetFacilities(string type = null)
        {
            try
            {
                IQueryable<Facility> query = _context.Facilities
                    .Include(f => f.Admin)
                        .ThenInclude(a => a.User);
                
                // Filter by facility type if specified
                if (!string.IsNullOrEmpty(type))
                {
                    query = query.Where(f => f.TypeOfFacility == type);
                }
                
                var facilities = await query.Select(f => new {
                    f.FacilityId,
                    f.TypeOfFacility,
                    f.FacilityCapacity,
                    f.FacilityImg,
                    f.FacilityName,
                    f.WorkingDays,
                    f.WorkingHours,
                    f.RulesRegulations,
                    f.Status,
                    f.AdminId,
                    CreatedBy = f.Admin != null && f.Admin.User != null 
                        ? $"{f.Admin.User.FirstName} {f.Admin.User.LastName}"
                        : "Admin"
                }).ToListAsync();
                
                return Ok(facilities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching facilities");
                return BadRequest(new { message = $"Failed to retrieve facilities: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("admin/facilities/counts")]
        public async Task<IActionResult> GetFacilityCounts()
        {
            try
            {
                var facilityCounts = await _context.Facilities
                    .GroupBy(f => f.TypeOfFacility)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToListAsync();
                    
                return Ok(facilityCounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching facility counts");
                return BadRequest(new { message = $"Failed to retrieve facility counts: {ex.Message}" });
            }
        }

        [HttpPost]
        [Route("admin/facilities/create")]
        public async Task<IActionResult> CreateFacility([FromForm] Facility facility, IFormFile facilityImage)
        {
            try
            {
                // Get admin ID from session
                var adminId = HttpContext.Session.GetInt32("AdminId");
                if (adminId == null)
                {
                    return Unauthorized(new { message = "Not authorized" });
                }
                
                if (facilityImage != null && facilityImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "facilities");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + facilityImage.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await facilityImage.CopyToAsync(fileStream);
                    }

                    facility.FacilityImg = "/uploads/facilities/" + uniqueFileName;
                }
                
                facility.Status = "Available"; // Set default status
                facility.AdminId = adminId; // Set the AdminId without .Value
                
                _context.Facilities.Add(facility);
                await _context.SaveChangesAsync();
                
                return Ok(new { message = "Facility created successfully", facility });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating facility");
                return BadRequest(new { message = $"Failed to create facility: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("admin/facilities/{id}")]
        public async Task<IActionResult> GetFacility(int id)
        {
            try
            {
                var facility = await _context.Facilities
                    .Include(f => f.Admin)
                        .ThenInclude(a => a.User)
                    .FirstOrDefaultAsync(f => f.FacilityId == id);
                
                if (facility == null)
                {
                    return NotFound(new { message = "Facility not found" });
                }
                
                var facilityDetails = new {
                    facility.FacilityId,
                    facility.TypeOfFacility,
                    facility.FacilityCapacity,
                    facility.FacilityImg,
                    facility.FacilityName,
                    facility.WorkingDays,
                    facility.WorkingHours,
                    facility.RulesRegulations,
                    facility.Status,
                    facility.AdminId,
                    CreatedBy = facility.Admin?.User != null 
                        ? $"{facility.Admin.User.FirstName} {facility.Admin.User.LastName}"
                        : "Admin"
                };
                
                return Ok(facilityDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching facility");
                return BadRequest(new { message = $"Failed to retrieve facility: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("admin/facilities/{id}")]
        public async Task<IActionResult> UpdateFacility(int id, [FromForm] Facility facility, IFormFile facilityImage)
        {
            try
            {
                var existingFacility = await _context.Facilities.FindAsync(id);
                
                if (existingFacility == null)
                {
                    return NotFound(new { message = "Facility not found" });
                }
                
                // Get admin ID from session
                var adminId = HttpContext.Session.GetInt32("AdminId");
                if (adminId == null)
                {
                    return Unauthorized(new { message = "Not authorized" });
                }
                
                // Update facility properties
                existingFacility.FacilityName = facility.FacilityName;
                existingFacility.TypeOfFacility = facility.TypeOfFacility;
                existingFacility.FacilityCapacity = facility.FacilityCapacity;
                existingFacility.WorkingDays = facility.WorkingDays;
                existingFacility.WorkingHours = facility.WorkingHours;
                existingFacility.RulesRegulations = facility.RulesRegulations;
                
                // Update facility image if provided
                if (facilityImage != null && facilityImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "facilities");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + facilityImage.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await facilityImage.CopyToAsync(fileStream);
                    }

                    // Delete old image file if it exists
                    if (!string.IsNullOrEmpty(existingFacility.FacilityImg))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingFacility.FacilityImg.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    existingFacility.FacilityImg = "/uploads/facilities/" + uniqueFileName;
                }
                
                await _context.SaveChangesAsync();
                
                return Ok(new { message = "Facility updated successfully", facility = existingFacility });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating facility");
                return BadRequest(new { message = $"Failed to update facility: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("admin/facilities/{id}/status")]
        public async Task<IActionResult> UpdateFacilityStatus(int id, [FromBody] string status)
        {
            try
            {
                var facility = await _context.Facilities.FindAsync(id);
                
                if (facility == null)
                {
                    return NotFound(new { message = "Facility not found" });
                }
                
                // Get admin ID from session
                var adminId = HttpContext.Session.GetInt32("AdminId");
                if (adminId == null)
                {
                    return Unauthorized(new { message = "Not authorized" });
                }
                
                // Validate status
                string[] validStatuses = { "Available", "Full Booked", "Maintenance", "Disabled" };
                if (!validStatuses.Contains(status))
                {
                    return BadRequest(new { message = "Invalid status. Valid statuses are: Available, Full Booked, Maintenance, Disabled" });
                }
                
                facility.Status = status;
                await _context.SaveChangesAsync();
                
                return Ok(new { message = "Facility status updated successfully", facility });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating facility status");
                return BadRequest(new { message = $"Failed to update facility status: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("admin/reservations/pending")]
        public async Task<IActionResult> GetPendingReservations()
        {
            try
            {
                var allReservations = await _context.Reservations
                    .Include(r => r.Facility)
                    .Include(r => r.Homeowner)
                        .ThenInclude(h => h.User)
                    .Select(r => new
                    {
                        reservationId = r.ReservationId,
                        facilityId = r.FacilityId,
                        facilityName = r.Facility.FacilityName,
                        homeownerId = r.HomeownerId,
                        homeownerName = $"{r.Homeowner.User.FirstName} {r.Homeowner.User.LastName}",
                        reservationDate = r.DateOfReservation.ToString("MM/dd/yy"),
                        timeIn = r.ReservationTimeIn.ToString("h:mm tt"),
                        timeOut = r.ReservationTimeOut.ToString("h:mm tt"),
                        capacity = r.Capacity,
                        facilityCapacity = r.Facility.FacilityCapacity,
                        status = r.Status
                    })
                    .ToListAsync();
                    
                return Ok(allReservations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reservations");
                return BadRequest(new { message = $"Failed to retrieve reservations: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("admin/reservations/{id}/approve")]
        public async Task<IActionResult> ApproveReservation(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                
                if (reservation == null)
                {
                    return NotFound(new { message = "Reservation not found" });
                }
                
                // Get admin ID from session
                var adminId = HttpContext.Session.GetInt32("AdminId");
                if (adminId == null)
                {
                    return Unauthorized(new { message = "Not authorized" });
                }
                
                reservation.Status = "Approved";
                reservation.AdminId = adminId.Value;
                
                await _context.SaveChangesAsync();
                
                return Ok(new { message = "Reservation approved successfully", reservation });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving reservation");
                return BadRequest(new { message = $"Failed to approve reservation: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("admin/reservations/{id}/reject")]
        public async Task<IActionResult> RejectReservation(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                
                if (reservation == null)
                {
                    return NotFound(new { message = "Reservation not found" });
                }
                
                // Get admin ID from session
                var adminId = HttpContext.Session.GetInt32("AdminId");
                if (adminId == null)
                {
                    return Unauthorized(new { message = "Not authorized" });
                }
                
                reservation.Status = "Rejected";
                reservation.AdminId = adminId.Value;
                
                await _context.SaveChangesAsync();
                
                return Ok(new { message = "Reservation rejected successfully", reservation });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting reservation");
                return BadRequest(new { message = $"Failed to reject reservation: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("admin/service-requests/list")]
        public async Task<IActionResult> GetServiceRequests()
        {
            try
            {
                var requests = await _context.ServiceRequests
                    .Include(sr => sr.Homeowner)
                        .ThenInclude(h => h.User)
                    .Include(sr => sr.Staff)
                        .ThenInclude(s => s.User)
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
                        HomeownerName = sr.Homeowner.User == null ? "Unknown" 
                            : sr.Homeowner.User.FirstName + " " + sr.Homeowner.User.LastName,
                        StaffName = sr.Staff.User == null ? null
                            : sr.Staff.User.FirstName + " " + sr.Staff.User.LastName
                    })
                    .ToListAsync();

                return Json(new { success = true, requests });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching service requests");
                return Json(new { success = false, message = "An error occurred while fetching service requests: " + ex.Message });
            }
        }

        [HttpPut]
        [Route("admin/service-requests/{id}/status")]
        public async Task<IActionResult> UpdateServiceRequestStatus(int id, [FromBody] JsonElement data)
        {
            try
            {
                var serviceRequest = await _context.ServiceRequests.FindAsync(id);
                if (serviceRequest == null)
                {
                    return Json(new { success = false, message = "Service request not found" });
                }

                if (serviceRequest.ServiceStatus == "Completed")
                {
                    return Json(new { success = false, message = "Cannot modify a completed service request" });
                }

                // Properly parse the status from the JSON payload
                if (!data.TryGetProperty("status", out JsonElement statusElement) || 
                    statusElement.ValueKind != JsonValueKind.String)
                {
                    return Json(new { success = false, message = "Invalid status provided" });
                }

                string status = statusElement.GetString() ?? "Pending";
                serviceRequest.ServiceStatus = status;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Service request status updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating service request status");
                return Json(new { success = false, message = "An error occurred while updating service request status: " + ex.Message });
            }
        }

        [HttpPut]
        [Route("admin/service-requests/{id}/assign")]
        public async Task<IActionResult> AssignServiceRequestStaff(int id, [FromBody] JsonElement data)
        {
            try
            {
                var serviceRequest = await _context.ServiceRequests.FindAsync(id);
                if (serviceRequest == null)
                {
                    return NotFound(new { message = "Service request not found" });
                }

                if (!data.TryGetProperty("staffId", out JsonElement staffIdElement))
                {
                    return BadRequest(new { message = "Staff ID is required" });
                }

                int staffId;
                if (staffIdElement.ValueKind == JsonValueKind.Number)
                {
                    staffId = staffIdElement.GetInt32();
                }
                else if (staffIdElement.ValueKind == JsonValueKind.String && int.TryParse(staffIdElement.GetString(), out int parsedId))
                {
                    staffId = parsedId;
                }
                else
                {
                    return BadRequest(new { message = "Invalid staff ID" });
                }

                var staff = await _context.Staffs.FindAsync(staffId);
                if (staff == null)
                {
                    return NotFound(new { message = "Staff not found" });
                }

                // Assign the staff to the service request
                serviceRequest.StaffId = staffId;
                // Optionally, set status to "Assigned"
                if (string.IsNullOrEmpty(serviceRequest.ServiceStatus) || serviceRequest.ServiceStatus == "Pending")
                {
                    serviceRequest.ServiceStatus = "Assigned";
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Service request assigned successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred: " + ex.Message });
            }
        }

        [HttpGet]
        [Route("admin/staff/list")]
        public async Task<IActionResult> GetStaffMembers()
        {
            try
            {
                var staff = await _context.Staffs
                    .Include(s => s.User)
                    .Select(s => new
                    {
                        s.StaffId,
                        Name = s.User.FirstName + " " + s.User.LastName,
                        Position = s.Position.ToString()
                    })
                    .ToListAsync();

                return Json(new { success = true, staff });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching staff members");
                return Json(new { success = false, message = "An error occurred while fetching staff members: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("admin/service-requests/update-status/{id}")]
        public async Task<IActionResult> UpdateServiceRequestStatus(int id, [FromBody] string newStatus)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null || serviceRequest.ServiceStatus == null)
            {
                return BadRequest("Invalid service request.");
            }

            if (serviceRequest.ServiceStatus == "Completed")
            {
                return BadRequest("Cannot modify a completed service request.");
            }

            try
            {
                serviceRequest.ServiceStatus = newStatus;
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Service request status updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("admin/service-requests/auto-assign")]
        public async Task<IActionResult> AutoAssignServiceRequests()
        {
            var unassignedRequests = await _context.ServiceRequests
                .Where(sr => sr.StaffId == null && sr.ServiceStatus != "Completed")
                .ToListAsync();

            var staffMembers = await _context.Staffs.ToListAsync();
            if (!staffMembers.Any())
            {
                return BadRequest("No staff members available for assignment.");
            }

            try
            {
                foreach (var request in unassignedRequests)
                {
                    var assignedStaff = staffMembers.OrderBy(s => Guid.NewGuid()).FirstOrDefault();
                    if (assignedStaff != null)
                    {
                        request.StaffId = assignedStaff.StaffId;
                        request.ServiceStatus = "Assigned";
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Unassigned service requests have been automatically assigned to staff." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Route("api/admin/feedback/list")]
        public async Task<IActionResult> GetFeedbackList()
        {
            try
            {
                var feedback = await _context.Feedbacks
                    .Include(f => f.Homeowner)
                        .ThenInclude(h => h.User)
                    .OrderByDescending(f => f.FeedbackDate)
                    .Select(f => new
                    {
                        f.FeedbackId,
                        f.Rating,
                        f.FeedbackContent,
                        f.FeedbackDate,
                        HomeownerName = f.Homeowner.User.FirstName + " " + f.Homeowner.User.LastName
                    })
                    .ToListAsync();

                return Ok(feedback);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching feedback list");
                return StatusCode(500, new { message = "An error occurred while retrieving feedback" });
            }
        }

        [HttpGet]
        [Route("api/admin/complaints/list")]
        public async Task<IActionResult> GetComplaintsList()
        {
            try
            {
                var complaints = await _context.Complaints
                    .Include(c => c.Homeowner)
                        .ThenInclude(h => h.User)
                    .OrderByDescending(c => c.ComplaintDate)
                    .Select(c => new
                    {
                        c.ComplaintId,
                        c.ComplaintContent,
                        c.ComplaintDate,
                        c.Status,
                        HomeownerName = c.Homeowner.User.FirstName + " " + c.Homeowner.User.LastName
                    })
                    .ToListAsync();

                return Ok(complaints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching complaints list");
                return StatusCode(500, new { message = "An error occurred while retrieving complaints" });
            }
        }

        [HttpPut]
        [Route("api/admin/complaints/{id}/status")]
        public async Task<IActionResult> UpdateComplaintStatus(int id, [FromBody] string status)
        {
            try
            {
                var complaint = await _context.Complaints.FindAsync(id);
                if (complaint == null)
                {
                    return NotFound(new { message = "Complaint not found" });
                }

                // Validate status
                string[] validStatuses = { "Pending", "In Progress", "Resolved", "Dismissed" };
                if (!validStatuses.Contains(status))
                {
                    return BadRequest(new { message = "Invalid status. Valid statuses are: Pending, In Progress, Resolved, Dismissed" });
                }

                complaint.Status = status;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Complaint status updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating complaint status");
                return StatusCode(500, new { message = "An error occurred while updating complaint status" });
            }
        }

        [Route("admin/billing")]
        public async Task<IActionResult> Billing()
        {
            ViewData["UserType"] = "Admin";
            ViewData["Page"] = "Billing";
            var bills = await _context.Bills
                .Include(b => b.Homeowner)
                    .ThenInclude(h => h.User)
                .Include(b => b.Payments)
                .OrderByDescending(b => b.DueDate)
                .ToListAsync();
            return View(bills);
        }

        [HttpPost]
        [Route("admin/billing/create")]
        public async Task<IActionResult> CreateBill([FromBody] Bill bill)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bill.Status = "Pending";
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Bill created successfully", bill });
        }

        [HttpGet]
        [Route("admin/billing/{id}")]
        public async Task<IActionResult> GetBill(int id)
        {
            var bill = await _context.Bills
                .Include(b => b.Homeowner)
                    .ThenInclude(h => h.User)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.BillId == id);

            if (bill == null)
            {
                return NotFound(new { message = "Bill not found" });
            }

            // Handle case where homeowner data might be missing
            var homeownerName = bill.Homeowner?.User != null
                ? new { firstName = bill.Homeowner.User.FirstName, lastName = bill.Homeowner.User.LastName }
                : new { firstName = "Unknown", lastName = "User" };

            var billDetails = new
            {
                billId = bill.BillId,
                homeowner = homeownerName,
                billType = bill.BillType,
                amount = bill.Amount,
                dueDate = bill.DueDate,
                status = bill.Status,
                amountPaid = bill.Payments.Sum(p => p.AmountPaid)
            };

            return Json(billDetails);
        }

        [HttpGet]
        [Route("admin/billing/{id}/payments")]
        public async Task<IActionResult> GetBillPayments(int id)
        {
            var bill = await _context.Bills
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.BillId == id);

            if (bill == null)
            {
                return NotFound(new { message = "Bill not found" });
            }

            var payments = bill.Payments.Select(p => new
            {
                paymentId = p.PaymentId,
                amountPaid = p.AmountPaid,
                modeOfPayment = p.ModeOfPayment,
                dateOfPayment = p.DateOfPayment
            });

            return Json(payments);
        }

        [HttpPut]
        [Route("admin/billing/{id}/status")]
        public async Task<IActionResult> UpdateBillStatus(int id, [FromBody] string status)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
            {
                return NotFound(new { message = "Bill not found" });
            }

            bill.Status = status;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Bill status updated successfully" });
        }

        [HttpGet]
        [Route("api/homeowners")]
        public async Task<IActionResult> GetHomeowners()
        {
            try
            {
                var homeowners = await _context.Homeowners
                    .Include(h => h.User)
                    .Select(h => new
                    {
                        homeownerId = h.HomeownerId,
                        firstName = h.User.FirstName,
                        lastName = h.User.LastName
                    })
                    .ToListAsync();

                return Json(homeowners);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching homeowners list");
                return StatusCode(500, new { message = "Error fetching homeowners" });
            }
        }
    }
}