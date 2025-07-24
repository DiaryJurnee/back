using API.Dtos.Clusters;
using Application.Clusters.Commands;
using Application.Common.Interfaces.Queries;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Domain.Clusters;
using Domain.Users;
using Domain.Workspaces;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClustersController(ISender sender, IBaseQuery<Cluster> clusterQuery) : ControllerBase
    {
        [HttpGet]
        public async Task<Result<Success, Error>> Get([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var clusterId = ClusterId.New(id);
            var result = await clusterQuery.Get(cancellationToken, x => x.Id == clusterId);

            return result.Match<Result<Success, Error>>(
                cluster => Success.Create(StatusCodes.Status200OK, ClusterDto.FromDomainModel(cluster)),
                () => Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("Cluster not found", Error.ServerErrorsKey))
            );
        }

        [HttpGet("[action]")]
        public async Task<Result<Success, Error>> GetByWorkspace([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var workspaceId = WorkspaceId.New(id);
            var result = await clusterQuery.GetMany(cancellationToken, x => x.WorkspaceId == workspaceId);

            return Success.Create(StatusCodes.Status200OK, result.Select(ClusterDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<Result<Success, Error>> GetByOwner([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var ownerId = UserId.New(id);
            var result = await clusterQuery.GetMany(cancellationToken, x => x.OwnerId == ownerId);

            return Success.Create(StatusCodes.Status200OK, result.Select(ClusterDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<Result<Success, Error>> GetByWorkspaceAndOwner([FromQuery] Guid workspaceId, [FromQuery] Guid ownerId, CancellationToken cancellationToken)
        {
            var workspace = WorkspaceId.New(workspaceId);
            var owner = UserId.New(ownerId);
            var result = await clusterQuery.GetMany(cancellationToken, x => x.WorkspaceId == workspace && x.OwnerId == owner);

            return Success.Create(StatusCodes.Status200OK, result.Select(ClusterDto.FromDomainModel));
        }

        [HttpPost]
        public async Task<Result<Success, Error>> Create([FromForm] CreateClusterDto dto, CancellationToken cancellationToken)
        {
            var input = new CreateClusterCommand
            {
                Name = dto.Name,
                WorkspaceId = dto.WorkspaceId,
                OwnerId = dto.OwnerId
            };

            var result = await sender.Send(input, cancellationToken);

            if (result.IsFailure)
                return result.Error;

            return Success.Create(StatusCodes.Status200OK, ClusterDto.FromDomainModel(result.Value));
        }

        [HttpPut]
        public async Task<Result<Success, Error>> Update([FromForm] UpdateClusterDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateClusterCommand
            {
                Id = dto.Id,
                Name = dto.Name
            };

            var result = await sender.Send(input, cancellationToken);

            if (result.IsFailure)
                return result.Error;

            return Success.Create(StatusCodes.Status200OK, ClusterDto.FromDomainModel(result.Value));
        }

        [HttpDelete]
        public async Task<Result<Success, Error>> Delete([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var input = new DeleteClusterCommand
            {
                Id = id
            };

            var result = await sender.Send(input, cancellationToken);

            if (result.IsFailure)
                return result.Error;

            return Success.Create(StatusCodes.Status200OK, ClusterDto.FromDomainModel(result.Value));
        }
    }
}
