using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SubdivisionApp.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

// ✅ Configure Kestrel to listen on HTTP (5002) and HTTPS (5001)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5002); // HTTP
    options.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps()); // HTTPS
});

// ✅ Database Context (EF Core with SQL Server)
builder.Services.AddDbContext<SubdivisionDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Add MVC services
builder.Services.AddControllersWithViews();

// ✅ Add Session Support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Add Distributed Memory Cache (Needed for Session)
builder.Services.AddDistributedMemoryCache();

// ✅ Register IHttpContextAccessor (Useful for session-based authentication)
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ✅ Configure Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // ✅ Place UseSession() BEFORE UseAuthorization()
app.UseAuthorization();

// ✅ Fix Default Route (Landing Page)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // Redirects to HomeController.Index

app.Run();
