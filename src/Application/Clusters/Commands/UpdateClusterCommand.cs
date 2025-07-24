using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Domain.Clusters;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Clusters.Commands;

public class UpdateClusterCommand : IRequest<Result<Cluster, Error>>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public class UpdateClusterCommandHandler(IBaseRepository<Cluster> clusterRepository, IBaseQuery<Cluster> clusterQuery) : IRequestHandler<UpdateClusterCommand, Result<Cluster, Error>>
{
    public async Task<Result<Cluster, Error>> Handle(UpdateClusterCommand request, CancellationToken cancellationToken)
    {
        var id = ClusterId.New(request.Id);
        var cluster = await clusterQuery.Get(cancellationToken, x => x.Id == id);

        return await cluster.Match<Task<Result<Cluster, Error>>>(async clusterToUpdate =>
            {
                clusterToUpdate.UpdateDetails(request.Name);
                return await clusterRepository.Update(clusterToUpdate, cancellationToken);
            },
            () => Task.FromResult(Result.Failure<Cluster, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("Cluster not found", Error.ServerErrorsKey))))
        );
    }
}
