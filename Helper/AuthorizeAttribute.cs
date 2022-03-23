using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Amigo.Models;
using Amigo.Entities;
namespace Amigo.Helpers;

[AttributeUsage(AttributeTargets.Method)]
public class AllowAnonymousAttribute : Attribute
{ }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly IList<Role> _roles;

    public AuthorizeAttribute(params Role[] roles)
    {
        _roles = roles ?? new Role[] { };
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata
            .OfType<AllowAnonymousAttribute>()
            .Any();
        if (allowAnonymous)
            return;

        var admin = (Admin)context.HttpContext.Items["User"]!;
        if (admin == null)
        {
            context.Result = new JsonResult(new { message = "unauthorized" })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }
        else if (!_roles.Contains(admin.Role) && _roles.Any())
        {
            context.Result = new JsonResult(new { message = "forbidden" })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}