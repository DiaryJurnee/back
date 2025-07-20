namespace Domain.Workspaces;

public record WorkspaceId(Guid Value)
{
    public static WorkspaceId New(Guid id) => new(id);
    public override string ToString() => Value.ToString();
}
