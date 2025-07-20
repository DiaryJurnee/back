using Domain.DayContents;

namespace Domain.Days;

public class Day
{
    private Day(DayId id, string title) =>
        (Id, Title) = (id, title);

    public DayId Id { get; }
    public string Title { get; private set; }

    public IEnumerable<DayContent> DayContents { get; } = [];

    public static Day New(DayId id, string title) =>
        new(id, title);
}
