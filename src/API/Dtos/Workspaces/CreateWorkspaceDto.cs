namespace API.Dtos.Workspaces;

public record CreateWorkspaceDto(string Name, Guid OwnerId);