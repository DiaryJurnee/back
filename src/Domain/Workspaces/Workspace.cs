using Domain.Users;
using Domain.UsersWorkspaces;

namespace Domain.Workspaces;

public class Workspace
{
    private Workspace(WorkspaceId id, string name, UserId ownerId) =>
        (Id, Name, OwnerId, CreatedAt) = (id, name, ownerId, DateTime.UtcNow);

    public WorkspaceId Id { get; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; }

    public UserId OwnerId { get; }
    public User? Owner { get; }

    public IEnumerable<UserWorkspace> Users { get; } = [];

    public static Workspace New(WorkspaceId id, string name, UserId ownerId) =>
        new(id, name, ownerId);

    public void UpdateDetails(string name) =>
        Name = name;
}
