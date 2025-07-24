namespace API.Dtos.UsersWorkspaces;

public record CreateUserWorkspaceDto(Guid UserId, Guid WorkspaceId, bool CanReadAll = true, bool CanCreate = true, bool CanUpdate = true, bool CanDelete = true, bool CanInviteOtherUser = false);
