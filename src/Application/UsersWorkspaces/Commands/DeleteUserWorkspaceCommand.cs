using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Domain.Users;
using Domain.UsersWorkspaces;
using Domain.Workspaces;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.UsersWorkspaces.Commands;

public class DeleteUserWorkspaceCommand : IRequest<Result<UserWorkspace, Error>>
{
    public Guid UserId { get; init; }
    public Guid WorkspaceId { get; init; }
}

public class DeleteUserWorkspaceCommandHandler(IBaseRepository<UserWorkspace> repository, IBaseQuery<UserWorkspace> userWorkspaceQuery) : IRequestHandler<DeleteUserWorkspaceCommand, Result<UserWorkspace, Error>>
{
    public async Task<Result<UserWorkspace, Error>> Handle(DeleteUserWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var userId = UserId.New(request.UserId);
        var workspaceId = WorkspaceId.New(request.WorkspaceId);

        var userWorkspace = await userWorkspaceQuery.Get(cancellationToken, x => x.WorkspaceId == workspaceId && x.UserId == userId);

        return await userWorkspace.Match<Task<Result<UserWorkspace, Error>>>(
            async userWorkspaceToDelete => await repository.Delete(userWorkspaceToDelete, cancellationToken),
            () => Task.FromResult(Result.Failure<UserWorkspace, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("User workspace not found", Error.ServerErrorsKey))))
        );
    }
}
