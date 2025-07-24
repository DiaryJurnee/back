using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Domain.Users;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Users.Commands;

public class UpdateUserCommand : IRequest<Result<User, Error>>
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}

public class UpdateUserCommandHandler(IBaseRepository<User> userRepository, IBaseQuery<User> userQuery) : IRequestHandler<UpdateUserCommand, Result<User, Error>>
{
    public async Task<Result<User, Error>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userId = UserId.New(request.Id);

        var user = await userQuery.Get(cancellationToken, x => x.Id == userId);

        return await user.Match<Task<Result<User, Error>>>(
            async userToUpdate =>
            {
                userToUpdate.UpdateDetails(request.FirstName, request.LastName);

                return await userRepository.Update(userToUpdate, cancellationToken);
            },
            () => Task.FromResult(Result.Failure<User, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("User not found", Error.ServerErrorsKey))))
        );
    }
}
