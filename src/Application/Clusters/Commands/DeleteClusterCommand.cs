using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Domain.Clusters;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Clusters.Commands;

public class DeleteClusterCommand : IRequest<Result<Cluster, Error>>
{
    public Guid Id { get; init; }
}

public class DeleteClusterCommandHandler(IBaseQuery<Cluster> clusterQuery, IBaseRepository<Cluster> clusterRepository) : IRequestHandler<DeleteClusterCommand, Result<Cluster, Error>>
{
    public async Task<Result<Cluster, Error>> Handle(DeleteClusterCommand request, CancellationToken cancellationToken)
    {
        var id = ClusterId.New(request.Id);
        var cluster = await clusterQuery.Get(cancellationToken, x => x.Id == id);

        return await cluster.Match<Task<Result<Cluster, Error>>>(
            async clusterToDelete => await clusterRepository.Delete(clusterToDelete, cancellationToken),
            () => Task.FromResult(Result.Failure<Cluster, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("Cluster not found", Error.ServerErrorsKey))))
        );
    }
}
