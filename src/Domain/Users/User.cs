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

    public ICollection<Workspace> Workspaces { get; } = [];
    public ICollection<UserWorkspace> UsersWorkspaces { get; } = [];
    public ICollection<Cluster> Clusters { get; } = [];

    public void UpdateDetails(string firstName, string lastName) =>
        (FirstName, LastName, UpdatedAt) = (firstName, lastName, DateTime.UtcNow);

    public void UpdateRole(SystemRoleId roleId) =>
        (RoleId, Role) = (roleId, null);

    public static User New(UserId id, string firstName, string lastName, string email, SystemRoleId roleId) =>
        new(id, firstName, lastName, email, roleId);
}
