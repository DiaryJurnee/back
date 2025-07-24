using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Common.Interfaces.Services;
using Application.Common.Settings.Jwt;
using Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using static Application.Common.Interfaces.Services.IJwtService;

namespace Application.Common.Services;

public class JwtService(IOptions<JwtSettings> options) : IJwtService
{
    private readonly JwtSettings _settings = options.Value;
    public async Task<string> GenerateToken(User user, CancellationToken cancellation)
    {
        return await Task.Run(() =>
        {
            var claims = new[]
            {
                new Claim(GetClaim(JwtClaimsType.UserId), user.Id.ToString()),
                new Claim(GetClaim(JwtClaimsType.FirstName), user.FirstName),
                new Claim(GetClaim(JwtClaimsType.LastName), user.LastName),
                new Claim(GetClaim(JwtClaimsType.Email), user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var creds = new SigningCredentials(key, _settings.SecurityAlgorithms);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }, cancellation);
    }

    public async Task<bool> ValidateToken(string token, CancellationToken cancellation)
    {
        try
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey)),
                ValidIssuer = _settings.Issuer,
                ValidAudience = _settings.Audience,
            };

            var result = await new JwtSecurityTokenHandler().ValidateTokenAsync(token, tokenValidationParameters);
            if (result.SecurityToken is JwtSecurityToken jwtSecurityToken)
                if (jwtSecurityToken.Header.Alg.Equals(_settings.SecurityAlgorithms, StringComparison.InvariantCultureIgnoreCase))
                    return true;
        }
        catch (SecurityTokenException)
        {
            return false;
        }
        return false;
    }
}
