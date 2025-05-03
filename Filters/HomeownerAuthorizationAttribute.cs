using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Subdivision.Models;

namespace Subdivision.Filters
{
    public class HomeownerAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userType = context.HttpContext.Session.GetInt32("UserType");
            var homeownerId = context.HttpContext.Session.GetInt32("HomeownerId");

            if (userType == null || homeownerId == null || (UserType)userType != UserType.Homeowner)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
