using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using Application.Common.Interfaces.Services;
using Application.Common.Settings.GoogleAuth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Common.Services;

public class GoogleTokenService(HttpClient http, IOptions<GoogleAuthSettings> options) : IGoogleTokenService
{
    private static GoogleKeysCache? _googleKeysCache = null;
    private readonly GoogleAuthSettings _settings = options.Value;

    public ClaimsPrincipal? Principal { get; private set; }

    private async Task FetchGoogleKeysAsync(CancellationToken cancellationToken = default)
    {
        var response = await http.GetAsync("https://www.googleapis.com/oauth2/v3/certs", cancellationToken);
        response.EnsureSuccessStatusCode();

        _googleKeysCache = await response.Content.ReadFromJsonAsync<GoogleKeysCache>(cancellationToken);

        if (_googleKeysCache is null)
            throw new Exception("Google keys not found after fetching");

        if (response.Headers.CacheControl != null && response.Headers.CacheControl.MaxAge.HasValue)
            _googleKeysCache.ExpiresAt = DateTimeOffset.UtcNow.Add(response.Headers.CacheControl.MaxAge.Value);
        else
            _googleKeysCache.ExpiresAt = DateTimeOffset.UtcNow.AddHours(6);
    }

    public async Task ValidateAsync(string tokenId, CancellationToken cancellationToken = default)
    {
        if (_googleKeysCache is null || _googleKeysCache.IsExpired)
            await FetchGoogleKeysAsync(cancellationToken);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(tokenId);

        var keyInfo = _googleKeysCache!.Keys.FirstOrDefault(k => k.Kid == token.Header.Kid);

        if (keyInfo == null)
        {
            await FetchGoogleKeysAsync(cancellationToken);
            keyInfo = _googleKeysCache!.Keys.FirstOrDefault(k => k.Kid == token.Header.Kid) ?? throw new Exception("Key not found after refresh attempt");
        }

        if (string.IsNullOrEmpty(keyInfo.N) || string.IsNullOrEmpty(keyInfo.E))
            throw new Exception("Invalid key parameters (N or E) found");

        var rsa = new RsaSecurityKey(new RSAParameters
        {
            Modulus = Base64UrlEncoder.DecodeBytes(keyInfo.N),
            Exponent = Base64UrlEncoder.DecodeBytes(keyInfo.E)
        });

        var validation = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://accounts.google.com",

            ValidateAudience = true,
            ValidAudience = _settings.ClientId,

            ValidateLifetime = true,
            IssuerSigningKey = rsa,

            ClockSkew = TimeSpan.FromMinutes(5)
        };

        try
        {
            var principal = handler.ValidateToken(tokenId, validation, out _);
            Principal = principal;
        }
        catch (SecurityTokenExpiredException)
        {
            throw new Exception("Token has expired", innerException: null);
        }
        catch (SecurityTokenValidationException ex)
        {
            throw new Exception("Token validation failed", ex);
        }
    }

    private class GoogleKeys(List<GoogleKey> keys)
    {
        public List<GoogleKey> Keys { get; set; } = keys;
    }

    private record GoogleKey(string Kid, string N, string E);

    private class GoogleKeysCache(List<GoogleKey> keys) : GoogleKeys(keys)
    {
        public DateTimeOffset ExpiresAt { get; set; } = DateTimeOffset.MinValue;
        public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
    }
}