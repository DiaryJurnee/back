using API.Dtos.UsersWorkspaces;
using Application.Common.Interfaces.Queries;
using Application.Common.Templates.Response;
using Application.UsersWorkspaces.Commands;
using CSharpFunctionalExtensions;
using Domain.Users;
using Domain.UsersWorkspaces;
using Domain.Workspaces;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersWorkspacesController(ISender sender, IBaseQuery<UserWorkspace> userWorkspaceQuery) : ControllerBase
    {
        [HttpGet]
        public async Task<Result<Success, Error>> Get([FromQuery] Guid userId, [FromQuery] Guid workspaceId, CancellationToken cancellationToken)
        {
            var userWorkspace = await userWorkspaceQuery.Get(cancellationToken, x => x.WorkspaceId == WorkspaceId.New(workspaceId) && x.UserId == UserId.New(userId));

            return userWorkspace.Match<Result<Success, Error>>(
                userWorkspace => Success.Create(StatusCodes.Status200OK, UserWorkspaceDto.FromDomainModel(userWorkspace)),
                () => Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("User workspace not found", Error.ServerErrorsKey))
            );
        }

        [HttpGet("[action]")]
        public async Task<Result<Success, Error>> GetByUserId([FromQuery] Guid userId, CancellationToken cancellationToken)
        {
            var user = UserId.New(userId);
            var result = await userWorkspaceQuery.GetMany(cancellationToken, x => x.UserId == user);

            return Success.Create(StatusCodes.Status200OK, result.Select(UserWorkspaceDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<Result<Success, Error>> GetByWorkspaceId([FromQuery] Guid workspaceId, CancellationToken cancellationToken)
        {
            var workspace = WorkspaceId.New(workspaceId);
            var result = await userWorkspaceQuery.GetMany(cancellationToken, x => x.WorkspaceId == workspace);

            return Success.Create(StatusCodes.Status200OK, result.Select(UserWorkspaceDto.FromDomainModel));
        }

        [HttpPost]
        public async Task<Result<Success, Error>> Create([FromForm] CreateUserWorkspaceDto dto, CancellationToken cancellationToken)
        {
            var input = new CreateUserWorkspaceCommand
            {
                WorkspaceId = dto.WorkspaceId,
                UserId = dto.UserId,
                CanReadAll = dto.CanReadAll,
                CanCreate = dto.CanCreate,
                CanUpdate = dto.CanUpdate,
                CanDelete = dto.CanDelete,
                CanInviteOtherUser = dto.CanInviteOtherUser
            };

            var result = await sender.Send(input, cancellationToken);

            if (result.IsFailure)
                return result.Error;

            return Success.Create(StatusCodes.Status200OK, UserWorkspaceDto.FromDomainModel(result.Value));
        }

        [HttpPut]
        public async Task<Result<Success, Error>> Update([FromForm] UpdateUserWorkspaceDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateUserWorkspaceCommand
            {
                WorkspaceId = dto.WorkspaceId,
                UserId = dto.UserId,
                CanReadAll = dto.CanReadAll,
                CanCreate = dto.CanCreate,
                CanUpdate = dto.CanUpdate,
                CanDelete = dto.CanDelete,
                CanInviteOtherUser = dto.CanInviteOtherUser
            };

            var result = await sender.Send(input, cancellationToken);

            if (result.IsFailure)
                return result.Error;

            return Success.Create(StatusCodes.Status200OK, UserWorkspaceDto.FromDomainModel(result.Value));
        }

        [HttpDelete]
        public async Task<Result<Success, Error>> Delete([FromQuery] Guid workspaceId, [FromQuery] Guid userId, CancellationToken cancellationToken)
        {
            var input = new DeleteUserWorkspaceCommand
            {
                WorkspaceId = workspaceId,
                UserId = userId
            };

            var result = await sender.Send(input, cancellationToken);

            if (result.IsFailure)
                return result.Error;

            return Success.Create(StatusCodes.Status200OK, UserWorkspaceDto.FromDomainModel(result.Value));
        }
    }
}
