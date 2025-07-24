using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Domain.Users;
using Domain.Workspaces;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Workspaces.Commands;

public class CreateWorkspaceCommand : IRequest<Result<Workspace, Error>>
{
    public string Name { get; init; } = string.Empty;
    public Guid OwnerId { get; init; }
}

public class CreateWorkspaceCommandHandler(IBaseRepository<Workspace> workspaceRepository, IBaseQuery<User> userQuery) : IRequestHandler<CreateWorkspaceCommand, Result<Workspace, Error>>
{
    public async Task<Result<Workspace, Error>> Handle(CreateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var ownerId = UserId.New(request.OwnerId);

        var user = await userQuery.Get(cancellationToken, x => x.Id == ownerId);

        return await user.Match<Task<Result<Workspace, Error>>>(async userToCreateWorkspaceFor =>
            {
                var workspace = Workspace.New(WorkspaceId.New(Guid.NewGuid()), request.Name, userToCreateWorkspaceFor.Id);

                var workspaceResult = await workspaceRepository.Create(workspace, cancellationToken);

                return workspaceResult;
            },
            () => Task.FromResult(Result.Failure<Workspace, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("User not found", Error.ServerErrorsKey))))
        );
    }
}
