using API.Dtos.Users;
using Domain.Workspaces;

namespace API.Dtos.Workspaces;

public record WorkspaceDto(Guid Id, string Name, DateTime CreatedAt, Guid OwnerId, UserDto? Owner)
{
    public static WorkspaceDto FromDomainModel(Workspace workspace) =>
        new(workspace.Id.Value, workspace.Name, workspace.CreatedAt, workspace.OwnerId.Value,
            workspace.Owner is null ? null : UserDto.FromDomainModel(workspace.Owner));
}
