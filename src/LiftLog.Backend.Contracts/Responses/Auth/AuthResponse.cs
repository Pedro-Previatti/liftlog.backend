using System.Diagnostics.CodeAnalysis;

namespace LiftLog.Backend.Contracts.Responses.Auth;

public class AuthResponse
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }

    [SetsRequiredMembers]
    private AuthResponse(string token, string refreshToken)
    {
        Token = token;
        RefreshToken = refreshToken;
    }

    public static AuthResponse New(string token, string refreshToken) => new(token, refreshToken);
}
