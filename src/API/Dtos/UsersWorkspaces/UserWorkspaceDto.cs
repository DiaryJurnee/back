using API.Dtos.Users;
using API.Dtos.Workspaces;
using Domain.UsersWorkspaces;

namespace API.Dtos.UsersWorkspaces;

public record UserWorkspaceDto(Guid UserId, Guid WorkspaceId,
    bool CanReadAll, bool CanCreate, bool CanUpdate, bool CanDelete, bool CanInviteOtherUser,
    DateTime CreatedAt, DateTime UpdatedAt,
    WorkspaceDto? Workspace, UserDto? User)
{
    public static UserWorkspaceDto FromDomainModel(UserWorkspace userWorkspace) =>
        new(userWorkspace.UserId.Value, userWorkspace.WorkspaceId.Value, userWorkspace.CanReadAll, userWorkspace.CanCreate, userWorkspace.CanUpdate, userWorkspace.CanDelete, userWorkspace.CanInviteOtherUser, userWorkspace.CreatedAt, userWorkspace.UpdatedAt,
            userWorkspace.Workspace is null ? null : WorkspaceDto.FromDomainModel(userWorkspace.Workspace),
            userWorkspace.User is null ? null : UserDto.FromDomainModel(userWorkspace.User));

}
