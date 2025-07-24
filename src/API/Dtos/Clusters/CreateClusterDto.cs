namespace API.Dtos.Clusters;

public record CreateClusterDto(string Name, Guid WorkspaceId, Guid OwnerId);
