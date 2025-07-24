using Domain.Users;
using Domain.Workspaces;

namespace Domain.UsersWorkspaces;

public class UserWorkspace
{
    private UserWorkspace(WorkspaceId workspaceId, UserId userId,
        bool canReadAll = true,
        bool canCreate = true,
        bool canUpdate = true,
        bool canDelete = true,
        bool canInviteOtherUser = false) =>
            (WorkspaceId, UserId, CreatedAt, UpdatedAt, CanReadAll, CanCreate, CanUpdate, CanDelete, CanInviteOtherUser) =
            (workspaceId, userId, DateTime.UtcNow, DateTime.UtcNow, canReadAll, canCreate, canUpdate, canDelete, canInviteOtherUser);

    public WorkspaceId WorkspaceId { get; }
    public Workspace? Workspace { get; }
    public UserId UserId { get; }
    public User? User { get; }

    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public bool CanReadAll { get; private set; }
    public bool CanCreate { get; private set; }
    public bool CanUpdate { get; private set; }
    public bool CanDelete { get; private set; }
    public bool CanInviteOtherUser { get; private set; }

    public static UserWorkspace New(WorkspaceId workspaceId, UserId userId, bool canReadAll, bool canCreate, bool canUpdate, bool canDelete, bool canInviteOtherUser) =>
        new(workspaceId, userId, canReadAll, canCreate, canUpdate, canDelete, canInviteOtherUser);

    public void UpdateDetails(bool canReadAll, bool canCreate, bool canUpdate, bool canDelete, bool canInviteOtherUser) =>
        (UpdatedAt, CanReadAll, CanCreate, CanUpdate, CanDelete, CanInviteOtherUser) = (DateTime.UtcNow, canReadAll, canCreate, canUpdate, canDelete, canInviteOtherUser);
}
