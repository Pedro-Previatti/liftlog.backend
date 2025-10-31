using System.Diagnostics.CodeAnalysis;

namespace LiftLog.Backend.Core.Entities;

public class Notification
{
    public required string Key { get; init; }
    public required string Message { get; init; }

    [SetsRequiredMembers]
    private Notification(string key, string message)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Notification Key cannot be null or empty", nameof(key));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException(
                "Notification Message cannot be null or empty",
                nameof(message)
            );

        Key = key;
        Message = message;
    }

    public static Notification New(string key, string message) => new(key, message);
}
