using LiftLog.Backend.Core.Entities;

namespace LiftLog.Backend.Tests.Core.Entities;

public class RefreshTokenTests
{
    private readonly Guid _userIdGuid = Guid.NewGuid();
    private readonly DateTime _expires = DateTime.UtcNow.AddDays(7);
    private const string Token = "WXBjV282TDV2UUM4eWR1Ujd1dGZOVFpZcXJHVU55YWM=";

    [Fact(DisplayName = "RefreshToken => New() Returns Success")]
    [Trait("Category", "Entity")]
    public void RefreshToken_New_ReturnsSuccess()
    {
        var refreshToken = RefreshToken.New(_userIdGuid, _expires, Token);

        Assert.NotNull(refreshToken);
        Assert.Equal(Token, refreshToken.Token);
        Assert.Equal(_userIdGuid, refreshToken.UserId);
        Assert.Equal(_expires, refreshToken.Expires);
        Assert.False(refreshToken.IsRevoked);
        Assert.False(refreshToken.IsUsed);
    }

    [Fact(DisplayName = "RefreshToken => New() Throws ArgumentException")]
    [Trait("Category", "Entity")]
    public void RefreshToken_New_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            RefreshToken.New(_userIdGuid, _expires, string.Empty)
        );
        Assert.Throws<ArgumentException>(() => RefreshToken.New(Guid.Empty, _expires, Token));
        Assert.Throws<ArgumentException>(() =>
            RefreshToken.New(_userIdGuid, DateTime.MinValue, Token)
        );
        Assert.Throws<ArgumentException>(() =>
            RefreshToken.New(_userIdGuid, DateTime.UtcNow, Token)
        );
    }

    [Fact(DisplayName = "RefreshToken => Revoke() Returns Success")]
    [Trait("Category", "Entity")]
    public void RefreshToken_Revoke_ReturnsSuccess()
    {
        var refreshToken = RefreshToken.New(_userIdGuid, _expires, Token);
        refreshToken.Revoke();

        Assert.NotNull(refreshToken);
        Assert.Equal(Token, refreshToken.Token);
        Assert.Equal(_userIdGuid, refreshToken.UserId);
        Assert.Equal(_expires, refreshToken.Expires);
        Assert.False(refreshToken.IsUsed);
        Assert.True(refreshToken.IsRevoked);
    }

    [Fact(DisplayName = "RefreshToken => Use() Returns Success")]
    [Trait("Category", "Entity")]
    public void RefreshToken_Use_ReturnsSuccess()
    {
        var refreshToken = RefreshToken.New(_userIdGuid, _expires, Token);
        refreshToken.Use();

        Assert.NotNull(refreshToken);
        Assert.Equal(Token, refreshToken.Token);
        Assert.Equal(_userIdGuid, refreshToken.UserId);
        Assert.Equal(_expires, refreshToken.Expires);
        Assert.False(refreshToken.IsRevoked);
        Assert.True(refreshToken.IsUsed);
    }
}
