namespace Domain.Days;

public record DayId(Guid Value)
{
    public static DayId New(Guid id) => new(id);
    public override string ToString() => Value.ToString();
}
