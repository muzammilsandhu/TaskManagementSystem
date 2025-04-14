using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

[AttributeUsage (AttributeTargets.Method | AttributeTargets.Class , AllowMultiple = true)]
public class PermissionAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
    private readonly string _permission;

    public PermissionAuthorizeAttribute ( string permission )
        {
        _permission = permission;
        }

    public void OnAuthorization ( AuthorizationFilterContext context )
        {
        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? true)
            {
            context.Result = new UnauthorizedResult ();
            return;
            }

        var hasPermission = user.Claims.Any (c => c.Type == "Permission" && c.Value == _permission);

        if (!hasPermission)
            {
            context.Result = new ForbidResult ();
            }
        }
    }
