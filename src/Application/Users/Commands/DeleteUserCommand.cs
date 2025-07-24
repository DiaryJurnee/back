using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Domain.Users;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Users.Commands;

public class DeleteUserCommand : IRequest<Result<User, Error>>
{
    public Guid Id { get; init; }
}

public class DeleteUserCommandHandler(IBaseRepository<User> repository, IBaseQuery<User> userQuery) : IRequestHandler<DeleteUserCommand, Result<User, Error>>
{
    public async Task<Result<User, Error>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userId = UserId.New(request.Id);

        var user = await userQuery.Get(cancellationToken, x => x.Id == userId);

        return await user.Match<Task<Result<User, Error>>>(
            async userToDelete => await repository.Delete(userToDelete, cancellationToken),
            () => Task.FromResult(Result.Failure<User, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("User not found", Error.ServerErrorsKey))))
        );
    }
}
