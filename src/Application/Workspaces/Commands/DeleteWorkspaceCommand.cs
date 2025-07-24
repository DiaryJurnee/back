using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Domain.Workspaces;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Workspaces.Commands;

public class DeleteWorkspaceCommand : IRequest<Result<Workspace, Error>>
{
    public Guid Id { get; init; }
}

public class DeleteWorkspaceCommandHandler(IBaseRepository<Workspace> workspaceRepository, IBaseQuery<Workspace> workspaceQuery) : IRequestHandler<DeleteWorkspaceCommand, Result<Workspace, Error>>
{
    public async Task<Result<Workspace, Error>> Handle(DeleteWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var id = WorkspaceId.New(request.Id);

        var workspace = await workspaceQuery.Get(cancellationToken, x => x.Id == id);

        return await workspace.Match<Task<Result<Workspace, Error>>>(
            async workspaceToDelete => await workspaceRepository.Delete(workspaceToDelete, cancellationToken),
            () => Task.FromResult(Result.Failure<Workspace, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("Workspace not found", Error.ServerErrorsKey))))
        );
    }
}
