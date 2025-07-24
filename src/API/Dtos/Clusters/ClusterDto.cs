using API.Dtos.Users;
using API.Dtos.Workspaces;
using Domain.Clusters;

namespace API.Dtos.Clusters;

public record ClusterDto(Guid Id, string Name, Guid WorkspaceId, Guid OwnerId, WorkspaceDto? Workspace, UserDto? Owner)
{
    public static ClusterDto FromDomainModel(Cluster cluster) => new(cluster.Id.Value, cluster.Name, cluster.WorkspaceId.Value, cluster.OwnerId.Value,
        cluster.Workspace is null ? null : WorkspaceDto.FromDomainModel(cluster.Workspace),
        cluster.Owner is null ? null : UserDto.FromDomainModel(cluster.Owner));
}
