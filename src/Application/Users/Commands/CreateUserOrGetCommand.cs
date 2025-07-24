using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Domain.SystemRoles;
using Domain.Users;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Users.Commands;

public class CreateUserOrGetCommand : IRequest<Result<User, Error>>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

public class CreateUserOrGetCommandHandler(IBaseQuery<SystemRole> systemRoleQuery, IBaseQuery<User> userQuery, IBaseRepository<User> userRepository) : IRequestHandler<CreateUserOrGetCommand, Result<User, Error>>
{
    public async Task<Result<User, Error>> Handle(CreateUserOrGetCommand request, CancellationToken cancellationToken)
    {
        var user = await userQuery.Get(cancellationToken, x => x.Email == request.Email);

        return await user.Match(
            user => Task.FromResult(Result.Success<User, Error>(user)),
            async () =>
            {
                var systemRoleOpt = await systemRoleQuery.Get(cancellationToken, x => x.Name == SystemRole.User);

                return await systemRoleOpt.Match<Task<Result<User, Error>>>(async systemRole =>
                {
                    var id = UserId.New(Guid.NewGuid());
                    var rawUser = User.New(id, request.FirstName, request.LastName, request.Email, systemRole.Id);

                    var user = await userRepository.Create(rawUser, cancellationToken);

                    return user;
                },
                () => Task.FromResult(Result.Failure<User, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("System role not found", Error.ServerErrorsKey)))));
            }
        );
    }
}
