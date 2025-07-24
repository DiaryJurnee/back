using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Domain.SystemRoles;
using Domain.Users;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Users.Commands;

public class UpdateUserRoleCommand : IRequest<Result<User, Error>>
{
    public Guid Id { get; init; }
    public Guid SystemRoleId { get; init; }
}

public class UpdateUserRoleCommandHandler(IBaseRepository<User> userRepository, IBaseQuery<User> userQuery, IBaseQuery<SystemRole> systemRoleQuery) : IRequestHandler<UpdateUserRoleCommand, Result<User, Error>>
{
    public async Task<Result<User, Error>> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var userId = UserId.New(request.Id);

        var user = await userQuery.Get(cancellationToken, x => x.Id == userId);

        return await user.Match(
            async userToUpdate =>
            {
                var systemRoleId = SystemRoleId.New(request.SystemRoleId);
                var systemRole = await systemRoleQuery.Get(cancellationToken, x => x.Id == systemRoleId);

                return await systemRole.Match<Task<Result<User, Error>>>(
                    async systemRoleToUpdate =>
                    {
                        userToUpdate.UpdateRole(systemRoleToUpdate.Id);
                        return await userRepository.Update(userToUpdate, cancellationToken);
                    },
                    () => Task.FromResult(Result.Failure<User, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("System role not found", Error.ServerErrorsKey))))
                );
            },
            () => Task.FromResult(Result.Failure<User, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("User not found", Error.ServerErrorsKey))))
        );
    }
}
