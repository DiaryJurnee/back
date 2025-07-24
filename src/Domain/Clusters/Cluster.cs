using Domain.Days;
using Domain.Users;
using Domain.Workspaces;

namespace Domain.Clusters;

public class Cluster
{
    private Cluster(ClusterId id, string name, WorkspaceId workspaceId, UserId ownerId) =>
        (Id, Name, CreatedAt, WorkspaceId, OwnerId) = (id, name, DateTime.UtcNow, workspaceId, ownerId);

    public ClusterId Id { get; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; set; }

    public WorkspaceId WorkspaceId { get; }
    public Workspace? Workspace { get; }
    public UserId OwnerId { get; }
    public User? Owner { get; }

    public ICollection<Day> Days { get; } = [];

    public static Cluster New(ClusterId id, string name, WorkspaceId workspaceId, UserId ownerId) =>
        new(id, name, workspaceId, ownerId);

    public void UpdateDetails(string name) =>
        Name = name;
}
