using LiftLog.Backend.Core.Entities;

namespace LiftLog.Backend.Tests.Core.Entities;

public class NotificationTests
{
    private const string ValidKey = "key";
    private const string ValidMessage = "message";

    [Fact(DisplayName = "Notification => New() Returns Success")]
    [Trait("Category", "Entity")]
    public void Notification_New_ReturnsSuccess()
    {
        var notification = Notification.New(ValidKey, ValidMessage);

        Assert.NotNull(notification);
        Assert.Equal(ValidKey, notification.Key);
        Assert.Equal(ValidMessage, notification.Message);
    }

    [Theory(DisplayName = "Notification => New() Throws ArgumentException")]
    [Trait("Category", "Entity")]
    [InlineData("", ValidMessage)]
    [InlineData(ValidKey, "")]
    public void Notification_New_ThrowsArgumentException(string key, string message)
    {
        Assert.Throws<ArgumentException>(() => Notification.New(key, message));
    }
}
