using API.Modules.Filters;
using Domain.SystemRoles;
using Microsoft.AspNetCore.Mvc;

namespace API.Attributes;

public class AuthorizationWorkspacePermissionAttribute : TypeFilterAttribute
{
    public AuthorizationWorkspacePermissionAttribute(WorkspacePermissionType permission, string role = SystemRole.User)
        : base(typeof(AuthorizationWorkspacePermissionFilter))
    {
        Arguments = [permission, role];
    }
}
