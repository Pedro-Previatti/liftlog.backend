using System.Diagnostics.CodeAnalysis;

namespace LiftLog.Backend.Core.Helpers;

public class Notification
{
    public required string Key { get; init; }
    public required string Message { get; init; }

    [SetsRequiredMembers]
    private Notification(string key, string message)
    {
        Key = key;
        Message = message;
    }

    public static Notification Add(string key, string message) => new(key, message);
}
