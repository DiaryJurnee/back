namespace API.Dtos.UsersWorkspaces;

public record UpdateUserWorkspaceDto(Guid UserId, Guid WorkspaceId, bool CanReadAll, bool CanCreate, bool CanUpdate, bool CanDelete, bool CanInviteOtherUser);
