using API.Dtos.Clusters;
using Domain.Days;

namespace API.Dtos.Days;

public record DayDto(Guid Id, string Title, DateTime CreatedAt, Guid ClusterId, ClusterDto? Cluster)
{
    public static DayDto FromDomainModel(Day day) =>
        new(day.Id.Value, day.Title, day.CreatedAt, day.ClusterId.Value,
            day.Cluster is null ? null : ClusterDto.FromDomainModel(day.Cluster));
}
