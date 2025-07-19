namespace Application.Common.Settings.Jwt;

public record JwtSettings(string Issuer, string Audience, string SecretKey, string SecurityAlgorithms, int ExpiryMinutes);
