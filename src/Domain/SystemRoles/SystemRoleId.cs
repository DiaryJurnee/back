namespace Domain.SystemRoles;

public record SystemRoleId(Guid Value)
{
    public static SystemRoleId New(Guid id) => new(id);
    public override string ToString() => Value.ToString();
}
