using API.Constants;
using Application.Common.Interfaces.Queries;
using Application.Common.Templates.Response;
using Domain.Users;
using Domain.UsersWorkspaces;
using Domain.Workspaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Modules.Filters;

public enum WorkspacePermissionType
{
    CanReadAll = 0,
    CanCreate,
    CanUpdate,
    CanDelete,
    CanInviteOtherUser,
}

// TODO: big thinking about not valid workspace for data in request
public class AuthorizationWorkspacePermissionFilter(
    IBaseQuery<UserWorkspace> userWorkspaceQuery,
    IBaseQuery<User> userQuery,
    WorkspacePermissionType permission,
    string role)
    : AuthorizationFilter(userQuery, role)
{
    public const string HeaderWorkspace = "X-Workspace";

    public override async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        await base.OnAuthorizationAsync(context);

        if (context.Result is not null)
            return;

        var headers = context.HttpContext.Request.Headers;

        if (!headers.TryGetValue(HeaderWorkspace, out var workspaceIdHeader))
        {
            context.Result = new ObjectResult(Error.Create(StatusCodes.Status405MethodNotAllowed,
                ErrorContent.Create("Headers: missing workspace id", Error.ServerErrorsKey)))
            {
                StatusCode = StatusCodes.Status405MethodNotAllowed
            };
            return;
        }

        if (!Guid.TryParse(workspaceIdHeader, out var workspaceGuidId))
        {
            context.Result = new ObjectResult(Error.Create(StatusCodes.Status400BadRequest,
                ErrorContent.Create("Headers: workspace id {0} is invalid", Error.ServerErrorsKey, workspaceIdHeader.ToString())))
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
            return;
        }

        var workspaceId = WorkspaceId.New(workspaceGuidId);

        if (context.HttpContext.Items[HttpContextKeys.CurrentUser] is not User user)
        {
            context.Result = new ObjectResult(Error.Create(StatusCodes.Status500InternalServerError,
                ErrorContent.Create("Programmer with two left hands", Error.ServerErrorsKey)))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            return;
        }

        var cancellationToken = context.HttpContext.RequestAborted;
        var userWorkspace = await userWorkspaceQuery.Get(cancellationToken, x => x.WorkspaceId == workspaceId && x.UserId == user.Id);

        bool isValid = true;
        userWorkspace.Match(
            userWorkspace =>
            {
                if (!HasPermission(userWorkspace, permission))
                {
                    context.Result = new ObjectResult(Error.Create(StatusCodes.Status403Forbidden,
                        ErrorContent.Create("Access denied: insufficient permissions", Error.ServerErrorsKey)))
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    };
                    isValid = false;
                    return;
                }

                context.HttpContext.Items.Add(HttpContextKeys.CurrentUserWorkspace, userWorkspace);
            },
            () =>
            {
                context.Result = new ObjectResult(Error.Create(StatusCodes.Status404NotFound,
                    ErrorContent.Create("Headers: user workspace not found", Error.ServerErrorsKey)))
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
                isValid = false;
            }
        );

        if (!isValid)
            return;
    }

    private static bool HasPermission(UserWorkspace userWorkspace, WorkspacePermissionType permission)
    {
        return permission switch
        {
            WorkspacePermissionType.CanReadAll => userWorkspace.CanReadAll,
            WorkspacePermissionType.CanCreate => userWorkspace.CanCreate,
            WorkspacePermissionType.CanUpdate => userWorkspace.CanUpdate,
            WorkspacePermissionType.CanDelete => userWorkspace.CanDelete,
            WorkspacePermissionType.CanInviteOtherUser => userWorkspace.CanInviteOtherUser,
            _ => false
        };
    }
}
