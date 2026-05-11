using AuthProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthProject.Utilities;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceProvider _serviceProvider;

    public PermissionHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // 1. Check if user is authenticated
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null) return;

        // 2. Get roles from the JWT claims
        var userRoles = context.User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

        using var scope = _serviceProvider.CreateScope();

        // 3. Create a scope to access the database (Handler is often Singleton/Transient, DbContext is Scoped)
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // 4. Check if any of the user's roles have the required permission
        var hasPermission = await dbContext.Roles
            .Where(r => userRoles.Contains(r.Name))
            .AnyAsync(r => r.RolePermissions.Any(rp => rp.Permission.Name == requirement.Permission));
    
        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}