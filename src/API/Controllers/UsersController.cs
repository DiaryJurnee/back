using API.Dtos.Users;
using Application.Common.Interfaces.Queries;
using Application.Common.Templates.Response;
using Application.Users.Commands;
using CSharpFunctionalExtensions;
using Domain.Users;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController(ISender sender, IBaseQuery<User> userQuery) : ControllerBase
    {
        [HttpGet]
        public async Task<Result<Success, Error>> Get([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var userId = UserId.New(id);
            var user = await userQuery.Get(cancellationToken, x => x.Id == userId, x => x.Include(x => x.Role!));

            return user.Match<Result<Success, Error>>(
                user => Success.Create(StatusCodes.Status200OK, UserDto.FromDomainModel(user)),
                () => Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("User not found", Error.ServerErrorsKey))
            );
        }

        [HttpDelete]
        public async Task<Result<Success, Error>> Delete([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var input = new DeleteUserCommand
            {
                Id = id
            };

            var result = await sender.Send(input, cancellationToken);

            if (result.IsFailure)
                return result.Error;

            return Success.Create(StatusCodes.Status200OK, UserDto.FromDomainModel(result.Value));
        }

        [HttpPut]
        public async Task<Result<Success, Error>> Update([FromForm] UpdateUserDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateUserCommand
            {
                Id = dto.Id,
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
