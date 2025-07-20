using Domain.Users;

namespace Domain.SystemRoles;

public class SystemRole
{
    private SystemRole(SystemRoleId id, string name) =>
        (Id, Name) = (id, name);

    public SystemRoleId Id { get; }
    public string Name { get; }

    public IEnumerable<User> Users { get; } = [];

    public static SystemRole New(SystemRoleId id, string name) =>
        new(id, name);
}
