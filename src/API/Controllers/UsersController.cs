using API.Attributes;
using API.Constants;
using API.Dtos.Users;
using Application.Common.Templates.Response;
using Application.Users.Commands;
using CSharpFunctionalExtensions;
using Domain.SystemRoles;
using Domain.Users;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [CustomAuthorize(SystemRole.User)]
    public class UsersController(ISender sender) : ControllerBase
    {
        [HttpGet]
        public Result<Success, Error> Get()
        {
            if (HttpContext.Items[HttpContextKeys.CurrentUser] is not User user)
                return Error.Create(StatusCodes.Status401Unauthorized, ErrorContent.Create("Unauthorized", Error.ServerErrorsKey));

            return Success.Create(StatusCodes.Status200OK, UserDto.FromDomainModel(user));
        }

        [HttpDelete]
        public async Task<Result<Success, Error>> Delete(CancellationToken cancellationToken)
        {
            if (HttpContext.Items[HttpContextKeys.CurrentUser] is not User user)
                return Error.Create(StatusCodes.Status401Unauthorized, ErrorContent.Create("Unauthorized", Error.ServerErrorsKey));

            var input = new DeleteUserCommand
            {
                Id = user.Id.Value
            };

            var result = await sender.Send(input, cancellationToken);

            if (result.IsFailure)
                return result.Error;

            return Success.Create(StatusCodes.Status200OK, UserDto.FromDomainModel(result.Value));
        }

        [HttpPut]
        public async Task<Result<Success, Error>> Update([FromForm] UpdateUserDto dto, CancellationToken cancellationToken)
        {
            if (HttpContext.Items[HttpContextKeys.CurrentUser] is not User user)
                return Error.Create(StatusCodes.Status401Unauthorized, ErrorContent.Create("Unauthorized", Error.ServerErrorsKey));

            var input = new UpdateUserCommand
            {

                Id = user.Id.Value,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
            };

            var result = await sender.Send(input, cancellationToken);

            if (result.IsFailure)
                return result.Error;

            return Success.Create(StatusCodes.Status200OK, UserDto.FromDomainModel(result.Value));
        }
    }
}
