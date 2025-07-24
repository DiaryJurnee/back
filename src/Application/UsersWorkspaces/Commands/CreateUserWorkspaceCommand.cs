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

public class CreateUserWorkspaceCommand : IRequest<Result<UserWorkspace, Error>>
{
    public Guid WorkspaceId { get; init; }
    public Guid UserId { get; init; }
    public bool CanReadAll { get; init; } = true;
    public bool CanCreate { get; init; } = true;
    public bool CanUpdate { get; init; } = true;
    public bool CanDelete { get; init; } = true;
    public bool CanInviteOtherUser { get; init; } = false;
}

public class CreateUserWorkspaceCommandHandler(
    IBaseRepository<UserWorkspace> repository,
    IBaseQuery<Workspace> workspaceQuery,
    IBaseQuery<User> userQuery,
    IBaseQuery<UserWorkspace> userWorkspaceQuery) : IRequestHandler<CreateUserWorkspaceCommand, Result<UserWorkspace, Error>>
{
    public async Task<Result<UserWorkspace, Error>> Handle(CreateUserWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspaceId = WorkspaceId.New(request.WorkspaceId);
        var userId = UserId.New(request.UserId);

        var userWorkspace = await userWorkspaceQuery.Get(cancellationToken, x => x.WorkspaceId == workspaceId && x.UserId == userId);

        return await userWorkspace.Match(
            userWorkspace => Task.FromResult(Result.Failure<UserWorkspace, Error>(Error.Create(StatusCodes.Status409Conflict, ErrorContent.Create("User workspace already exists", Error.ServerErrorsKey)))),
            async () =>
            {
                var workspace = await workspaceQuery.Get(cancellationToken, x => x.Id == workspaceId);

                return await workspace.Match(async workspaceToCreateUserWorkspaceCommandFor =>
                    {
                        var userId = UserId.New(request.UserId);

                        var user = await userQuery.Get(cancellationToken, x => x.Id == userId);

                        return await user.Match<Task<Result<UserWorkspace, Error>>>(async userToCreateUserWorkspaceCommandFor =>
                            {
                                var userWorkspace = UserWorkspace.New(workspaceToCreateUserWorkspaceCommandFor.Id, userToCreateUserWorkspaceCommandFor.Id, request.CanReadAll, request.CanCreate, request.CanUpdate, request.CanDelete, request.CanInviteOtherUser);

                                return await repository.Create(userWorkspace, cancellationToken);
                            },
                            () => Task.FromResult(Result.Failure<UserWorkspace, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("User not found", Error.ServerErrorsKey))))
                        );
                    },
                    () => Task.FromResult(Result.Failure<UserWorkspace, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("Workspace not found", Error.ServerErrorsKey))))
                );
            }
        );
    }
}
