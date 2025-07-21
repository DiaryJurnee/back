using Domain.Clusters;
using Domain.DayContents;

namespace Domain.Days;

public class Day
{
    private Day(DayId id, string title, ClusterId clusterId) =>
        (Id, Title, ClusterId) = (id, title, clusterId);

    public DayId Id { get; }
    public string Title { get; private set; }

    public ClusterId ClusterId { get; }
    public Cluster? Cluster { get; }

    public IEnumerable<DayContent> DayContents { get; } = [];

    public static Day New(DayId id, string title, ClusterId clusterId) =>
        new(id, title, clusterId);

    public void UpdateDetails(string title) => Title = title;
}
