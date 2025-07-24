using API.Dtos.SystemRoles;
using Domain.Users;

namespace API.Dtos.Users;

public record UserDto(Guid Id, string Email, string FirstName, string LastName, DateTime CreatedAt, DateTime UpdatedAt, Guid RoleId, SystemRoleDto? Role)
{
    public static UserDto FromDomainModel(User user) =>
        new(user.Id.Value, user.Email, user.FirstName, user.LastName, user.CreatedAt, user.UpdatedAt, user.RoleId.Value,
            user.Role is null ? null : SystemRoleDto.FromDomainModel(user.Role));
}
