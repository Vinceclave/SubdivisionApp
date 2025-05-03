using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Subdivision.Models;

namespace Subdivision.Filters
{
    public class StaffAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userType = context.HttpContext.Session.GetInt32("UserType");
            var staffId = context.HttpContext.Session.GetInt32("StaffId");

            if (userType == null || staffId == null || (UserType)userType != UserType.Staff)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
