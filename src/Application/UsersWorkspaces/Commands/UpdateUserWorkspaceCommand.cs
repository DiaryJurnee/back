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

public class UpdateUserWorkspaceCommand : IRequest<Result<UserWorkspace, Error>>
{
    public Guid UserId { get; init; }
    public Guid WorkspaceId { get; init; }
    public bool CanReadAll { get; init; }
    public bool CanCreate { get; init; }
    public bool CanUpdate { get; init; }
    public bool CanDelete { get; init; }
    public bool CanInviteOtherUser { get; init; }
}

public class UpdateUserWorkspaceCommandHandler(IBaseRepository<UserWorkspace> repository, IBaseQuery<UserWorkspace> userWorkspaceQuery) : IRequestHandler<UpdateUserWorkspaceCommand, Result<UserWorkspace, Error>>
{
    public async Task<Result<UserWorkspace, Error>> Handle(UpdateUserWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var userId = UserId.New(request.UserId);
        var workspaceId = WorkspaceId.New(request.WorkspaceId);
        var userWorkspace = await userWorkspaceQuery.Get(cancellationToken, x => x.WorkspaceId == workspaceId && x.UserId == userId);

        return await userWorkspace.Match<Task<Result<UserWorkspace, Error>>>(async userWorkspaceToUpdate =>
            {
                userWorkspaceToUpdate.UpdateDetails(request.CanReadAll, request.CanCreate, request.CanUpdate, request.CanDelete, request.CanInviteOtherUser);
                return await repository.Update(userWorkspaceToUpdate, cancellationToken);
            },
            () => Task.FromResult(Result.Failure<UserWorkspace, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("User workspace not found", Error.ServerErrorsKey))))
        );
    }
}
