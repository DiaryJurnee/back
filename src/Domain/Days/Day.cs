using Domain.Clusters;
using Domain.DayContents;

namespace Domain.Days;

public class Day
{
    private Day(DayId id, string title, ClusterId clusterId, DateTime createdAt) =>
        (Id, Title, ClusterId, CreatedAt) = (id, title, clusterId, createdAt);

    public DayId Id { get; }
    public string Title { get; private set; }
    public DateTime CreatedAt { get; }

    public ClusterId ClusterId { get; }
    public Cluster? Cluster { get; }

    public ICollection<DayContent> DayContents { get; } = [];

    public static Day New(DayId id, string title, ClusterId clusterId) =>
        new(id, title, clusterId, DateTime.UtcNow);

    public void UpdateDetails(string title) => Title = title;
}
