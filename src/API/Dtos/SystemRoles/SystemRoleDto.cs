using Domain.SystemRoles;

namespace API.Dtos.SystemRoles;

public record SystemRoleDto(Guid Id, string Name)
{
    public static SystemRoleDto FromDomainModel(SystemRole systemRole) =>
        new(systemRole.Id.Value, systemRole.Name);
}
