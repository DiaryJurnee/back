using Domain.Clusters;
using Domain.SystemRoles;
using Domain.UsersWorkspaces;
using Domain.Workspaces;

namespace Domain.Users;

public class User
{
    private User(UserId id, string firstName, string lastName, string email, SystemRoleId roleId) =>
        (Id, FirstName, LastName, Email, RoleId, CreatedAt, UpdatedAt) = (id, firstName, lastName, email, roleId, DateTime.UtcNow, DateTime.UtcNow);

    public UserId Id { get; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }

    public SystemRoleId RoleId { get; private set; }
    public SystemRole? Role { get; private set; }

    public IEnumerable<Workspace> Workspaces { get; } = [];
    public IEnumerable<UserWorkspace> UsersWorkspaces { get; } = [];
    public IEnumerable<Cluster> Clusters { get; } = [];

    public void UpdateDetails(string firstName, string lastName, SystemRoleId roleId) =>
        (FirstName, LastName, RoleId, Role, UpdatedAt) = (firstName, lastName, roleId, null, DateTime.UtcNow);

    public static User New(UserId id, string firstName, string lastName, string email, SystemRoleId roleId) =>
        new(id, firstName, lastName, email, roleId);
}
