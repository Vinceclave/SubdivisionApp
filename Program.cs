using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Subdivision.Data;
using Subdivision.Models;
using Subdivision.Middleware;

namespace Subdivision
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews();

            // Configure session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Register the ApplicationDbContext with the connection string and enable retry logic
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<SubdivisionDbContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions =>
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,  // Max retry attempts
                        maxRetryDelay: TimeSpan.FromSeconds(10),  // Max delay between retries
                        errorNumbersToAdd: null  // Optionally, specify error codes to trigger retries (null retries on all transient errors)
                    )
                )
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseAuthMiddleware();

            // Set default route to Home/Index
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Ensure login route is handled after default route
            app.MapControllerRoute(
                name: "login",
                pattern: "login",
                defaults: new { controller = "Auth", action = "Login" });

            // Run database migrations and seed admin user
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    var context = services.GetRequiredService<SubdivisionDbContext>();
                    
                    // Apply pending migrations automatically
                    await context.Database.MigrateAsync();

                    // Seed admin user if not exists
                    await SeedAdminUser(context, logger);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while initializing the database.");
                }
            }

            await app.RunAsync();
        }

        /// <summary>
        /// Creates an admin user if one does not exist.
        /// </summary>
        private static async Task SeedAdminUser(SubdivisionDbContext context, ILogger logger)
        {
            if (!await context.Users.AnyAsync(u => u.UserType == UserType.Admin))
            {
                string adminPassword = "admin123";
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(adminPassword);

                var adminUser = new User
                {
                    Username = "admin",
                    Password = hashedPassword,
                    FirstName = "System",
                    LastName = "Administrator",
                    Email = "admin@subdivision.com",
                    UserType = UserType.Admin,
                    Address = "System Address",
                    PhoneNumber = "1234567890"
                };

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();

                var admin = new Admin
                {
                    LoginId = adminUser.LoginId
                };

                await context.Admins.AddAsync(admin);
                await context.SaveChangesAsync();

                logger.LogInformation("Admin user created successfully!");
            }
            else
            {
                logger.LogInformation("Admin user already exists. Skipping creation.");
            }
        }
    }
}
