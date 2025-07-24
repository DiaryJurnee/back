using System.Security.Claims;

namespace Application.Common.Interfaces.Services;

public interface IGoogleTokenService
{
    ClaimsPrincipal? Principal { get; }
    Task ValidateAsync(string tokenId, CancellationToken cancellationToken = default);
}
