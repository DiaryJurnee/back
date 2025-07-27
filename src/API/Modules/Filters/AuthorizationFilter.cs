using API.Constants;
using Application.Common.Interfaces.Queries;
using Application.Common.Templates.Response;
using Domain.SystemRoles;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using static Application.Common.Interfaces.Services.IJwtService;

namespace API.Modules.Filters;

public class AuthorizationFilter(IBaseQuery<User> userQuery, string role) : IAsyncAuthorizationFilter
{
    public async virtual Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new ObjectResult(Error.Create(StatusCodes.Status401Unauthorized, ErrorContent.Create("Unauthorized", Error.ServerErrorsKey)))
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        var userIdClaim = user.Claims.FirstOrDefault(x => x.Type == GetClaim(JwtClaimsType.UserId));
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userGuidId))
        {
            context.Result = new ObjectResult(Error.Create(StatusCodes.Status401Unauthorized, ErrorContent.Create("Unauthorized", Error.ServerErrorsKey)))
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        var userId = UserId.New(userGuidId);
        var cancellationToken = context.HttpContext.RequestAborted;
        var fetchedUser = await userQuery.Get(cancellationToken, x => x.Id == userId, x => x.Include(x => x.Role!));

        bool isValid = true;
        fetchedUser.Match(
            user =>
            {
                if (user.Role!.Name != role && user.Role.Name != SystemRole.Admin)
                {
                    context.Result = new ObjectResult(Error.Create(StatusCodes.Status403Forbidden, ErrorContent.Create("Forbidden", Error.ServerErrorsKey)))
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    };
                    isValid = false;
                    return;
                }

                context.HttpContext.Items.Add(HttpContextKeys.CurrentUser, user);
            },
            () =>
            {
                context.Result = new ObjectResult(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("User in token not found", Error.ServerErrorsKey)))
                {
                    StatusCode = StatusCodes.Status404NotFound
                };
                isValid = false;
                return;
            }
        );

        if (!isValid)
            return;
    }
}
