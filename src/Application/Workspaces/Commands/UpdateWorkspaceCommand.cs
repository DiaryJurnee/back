using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Domain.Workspaces;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Workspaces.Commands;

public class UpdateWorkspaceCommand : IRequest<Result<Workspace, Error>>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public class UpdateWorkspaceCommandHandler(IBaseRepository<Workspace> workspaceRepository, IBaseQuery<Workspace> workspaceQuery) : IRequestHandler<UpdateWorkspaceCommand, Result<Workspace, Error>>
{
    public async Task<Result<Workspace, Error>> Handle(UpdateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var id = WorkspaceId.New(request.Id);
        var workspace = await workspaceQuery.Get(cancellationToken, x => x.Id == id);

        return await workspace.Match<Task<Result<Workspace, Error>>>(async workspaceToUpdate =>
            {
                workspaceToUpdate.UpdateDetails(request.Name);

                return await workspaceRepository.Update(workspaceToUpdate, cancellationToken);
            },
            () => Task.FromResult(Result.Failure<Workspace, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("Workspace not found", Error.ServerErrorsKey))))
        );
    }
}
