namespace Domain.Clusters;

public record ClusterId(Guid Value)
{
    public static ClusterId New(Guid id) => new(id);
    public static ClusterId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
