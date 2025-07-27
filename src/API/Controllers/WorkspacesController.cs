using API.Attributes;
using API.Dtos.Workspaces;
using Application.Common.Interfaces.Queries;
using Application.Common.Templates.Response;
using Application.Workspaces.Commands;
using CSharpFunctionalExtensions;
using Domain.SystemRoles;
using Domain.Users;
using Domain.Workspaces;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [CustomAuthorize(SystemRole.User)]
    public class WorkspacesController(ISender sender, IBaseQuery<Workspace> workspaceQuery) : ControllerBase
    {
        [HttpGet]
        public async Task<Result<Success, Error>> Get([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var workspaceId = WorkspaceId.New(id);

            var workspace = await workspaceQuery.Get(cancellationToken, x => x.Id == workspaceId);

            return workspace.Match<Result<Success, Error>>(
                workspace => Success.Create(StatusCodes.Status200OK, WorkspaceDto.FromDomainModel(workspace)),
                () => Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("Workspace not found", Error.ServerErrorsKey))
            );
        }

        [HttpGet("[action]")]
        public async Task<Result<Success, Error>> GetByOwner([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var ownerId = UserId.New(id);

            var workspaces = await workspaceQuery.GetMany(cancellationToken, x => x.OwnerId == ownerId);

            return Success.Create(StatusCodes.Status200OK, workspaces.Select(WorkspaceDto.FromDomainModel));
        }

        [HttpPost]
        public async Task<Result<Success, Error>> Create([FromForm] CreateWorkspaceDto dto, CancellationToken cancellationToken)
        {
            var input = new CreateWorkspaceCommand
            {
                Name = dto.Name,
                OwnerId = dto.OwnerId
            };

            var result = await sender.Send(input, cancellationToken);

            if (result.IsFailure)
                return result.Error;

            return Success.Create(StatusCodes.Status200OK, WorkspaceDto.FromDomainModel(result.Value));
        }

        [HttpPut]
        public async Task<Result<Success, Error>> Update([FromForm] UpdateWorkspaceDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateWorkspaceCommand
            {
                Id = dto.Id,
                Name = dto.Name
            };

            var result = await sender.Send(input, cancellationToken);

            if (result.IsFailure)
                return result.Error;

            return Success.Create(StatusCodes.Status200OK, WorkspaceDto.FromDomainModel(result.Value));
        }

        [HttpDelete]
        public async Task<Result<Success, Error>> Delete([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var input = new DeleteWorkspaceCommand
            {
                Id = id
            };

            var result = await sender.Send(input, cancellationToken);

            if (result.IsFailure)
                return result.Error;

            return Success.Create(StatusCodes.Status200OK, WorkspaceDto.FromDomainModel(result.Value));
        }
    }
}
