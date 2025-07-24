using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Domain.Clusters;
using Domain.Users;
using Domain.Workspaces;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Clusters.Commands;

public class CreateClusterCommand : IRequest<Result<Cluster, Error>>
{
    public string Name { get; init; } = string.Empty;
    public Guid WorkspaceId { get; init; }
    public Guid OwnerId { get; init; }
}

public class CreateClusterCommandHandler(IBaseRepository<Cluster> clusterRepository, IBaseQuery<Workspace> workspaceQuery, IBaseQuery<User> userQuery) : IRequestHandler<CreateClusterCommand, Result<Cluster, Error>>
{
    public async Task<Result<Cluster, Error>> Handle(CreateClusterCommand request, CancellationToken cancellationToken)
    {
        var workspaceId = WorkspaceId.New(request.WorkspaceId);
        var workspace = await workspaceQuery.Get(cancellationToken, x => x.Id == workspaceId);

        return await workspace.Match(async workspaceToCreateClusterFor =>
            {
                var userId = UserId.New(request.OwnerId);

                var user = await userQuery.Get(cancellationToken, x => x.Id == userId);

                return await user.Match<Task<Result<Cluster, Error>>>(async userToCreateClusterFor =>
                    {
                        var cluster = Cluster.New(ClusterId.New(Guid.NewGuid()), request.Name, workspaceToCreateClusterFor.Id, userToCreateClusterFor.Id);

                        var clusterResult = await clusterRepository.Create(cluster, cancellationToken);

                        return clusterResult;
                    },
                    () => Task.FromResult(Result.Failure<Cluster, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("User not found", Error.ServerErrorsKey))))
                );
            },
            () => Task.FromResult(Result.Failure<Cluster, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("Workspace not found", Error.ServerErrorsKey))))
        );
    }
}
