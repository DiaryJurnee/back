namespace Domain.Users;

public record UserId(Guid Value)
{
    public static UserId New(Guid id) => new(id);
    public override string ToString() => Value.ToString();
}
