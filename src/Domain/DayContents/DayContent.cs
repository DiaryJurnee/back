using Domain.Days;

namespace Domain.DayContents;

public class DayContent
{
    private DayContent(DayContentId id, string text, DateTime? startAt, DateTime? endAt, DayId dayId) =>
        (Id, Text, StartAt, EndAt, CreatedAt, UpdatedAt, DayId) = (id, text, startAt, endAt, DateTime.UtcNow, DateTime.UtcNow, dayId);

    public DayContentId Id { get; }
    public string Text { get; private set; }
    public DateTime? StartAt { get; private set; }
    public DateTime? EndAt { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }

    public DayId DayId { get; }
    public Day? Day { get; }

    public void UpdateDetails(string text, DateTime? startAt, DateTime? endAt) =>
        (Text, StartAt, EndAt, UpdatedAt) = (text, startAt, endAt, DateTime.UtcNow);

    public static DayContent New(DayContentId id, string text, DateTime? startAt, DateTime? endAt, DayId dayId) =>
        new(id, text, startAt, endAt, dayId);
}
