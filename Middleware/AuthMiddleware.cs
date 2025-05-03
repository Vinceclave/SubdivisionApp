using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Subdivision.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _publicPaths = new[] { "/", "/home", "/login" };
        private readonly string[] _restrictedPaths = new[] { "/admin", "/dashboard", "/homeowner", "/staff" };

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path.Value?.ToLower() ?? "";
            bool isAuthenticated = context.Session.GetInt32("UserId") != null;
            
            // Check if trying to access restricted paths
            if (_restrictedPaths.Any(p => path.StartsWith(p)))
            {
                if (!isAuthenticated)
                {
                    context.Response.Redirect("/login");
                    return;
                }
            }

            // Always allow access to public paths
            if (_publicPaths.Any(p => path.StartsWith(p)))
            {
                await _next(context);
                return;
            }

            // For any other paths, require authentication
            if (!isAuthenticated)
            {
                context.Response.Redirect("/login");
                return;
            }

            await _next(context);
        }
    }

    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
