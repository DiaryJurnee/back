using Domain.Users;

namespace Application.Common.Interfaces.Services;

public interface IJwtService
{
    Task<string> GenerateToken(User user, CancellationToken cancellation);
    Task<bool> ValidateToken(string token, CancellationToken cancellation);

    enum JwtClaimsType { UserId = 0, FirstName = 1, LastName, Email };

    private static readonly Dictionary<JwtClaimsType, string> Claims = new()
    {
        { JwtClaimsType.UserId, "userId" },
        { JwtClaimsType.FirstName, "firstName" },
        { JwtClaimsType.LastName, "lastName" },
        { JwtClaimsType.Email, "email" },
    };

    public static string GetClaim(JwtClaimsType type) => Claims[type];
}
