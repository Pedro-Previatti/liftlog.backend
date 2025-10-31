using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LiftLog.Backend.Core.Entities;
using LiftLog.Backend.Core.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LiftLog.Backend.Core.Helpers;

public class TokenHelper(IOptions<JwtOptions> options)
{
    private readonly JwtOptions _options = options.Value;
    private readonly JwtSecurityTokenHandler _handler = new();

    public string GenerateJwtToken(User user) =>
        _handler.WriteToken(
            new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims:
                [
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName),
                    new Claim("cpf", user.Cpf),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(
                        JwtRegisteredClaimNames.Iat,
                        DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                        ClaimValueTypes.Integer64
                    ),
                ],
                expires: DateTime.UtcNow.AddDays(_options.ExpirationDays),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
                    SecurityAlgorithms.HmacSha256
                )
            )
        );

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string expiredToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
            ValidateLifetime = false, // Important: we want to get principal from expired token
        };

        try
        {
            var principal = _handler.ValidateToken(
                expiredToken,
                tokenValidationParameters,
                out var securityToken
            );

            if (
                securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase
                )
            )
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    public string? GetUserIdFromToken(string token) =>
        GetPrincipalFromExpiredToken(token)?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? GetUserEmailFromToken(string token) =>
        GetPrincipalFromExpiredToken(token)?.FindFirst(ClaimTypes.Email)?.Value;

    public RefreshToken GenerateRefreshToken(User user) =>
        RefreshToken.New(
            user.Id,
            DateTime.UtcNow.AddDays(_options.ExpirationDays),
            Convert
                .ToBase64String(RandomNumberGenerator.GetBytes(64))
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=')
        );
}
