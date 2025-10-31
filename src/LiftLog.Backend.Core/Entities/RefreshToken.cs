namespace LiftLog.Backend.Core.Entities;

public class RefreshToken
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public Guid UserId { get; private init; }

    public DateTime Expires { get; private init; }

    public string Token { get; private init; }

    public bool IsRevoked { get; private set; }
    public bool IsUsed { get; private set; }

    private RefreshToken(Guid userId, DateTime expires, string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException(
                "RefreshToken Token cannot be null or empty",
                nameof(token)
            );

        if (userId.Equals(Guid.Empty))
            throw new ArgumentException("RefreshToken UserId cannot be empty", nameof(userId));

        if (expires <= DateTime.UtcNow)
            throw new ArgumentException(
                "RefreshToken Expires cannot be in the past",
                nameof(expires)
            );

        UserId = userId;
        Expires = expires;
        Token = token;
    }

    public static RefreshToken New(Guid userId, DateTime expires, string token) =>
        new(userId, expires, token);

    public void Revoke() => IsRevoked = true;

    public void Use() => IsUsed = true;

    private bool IsExpired() => Expires <= DateTime.UtcNow;

    public bool IsValid() => !IsRevoked && !IsUsed && !IsExpired();
}
