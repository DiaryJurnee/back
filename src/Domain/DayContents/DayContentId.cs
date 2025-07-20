namespace Domain.DayContents;

public record DayContentId(Guid Value)
{
    public static DayContentId New(Guid id) => new(id);
    public override string ToString() => Value.ToString();
}
