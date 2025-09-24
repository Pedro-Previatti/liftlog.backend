using FluentValidation.Results;
using LiftLog.Backend.Core.Entities;

namespace LiftLog.Backend.Core.Helpers;

public class NotificationContext
{
    private readonly List<Notification> _notifications = new();
    public IReadOnlyList<Notification> Notifications => _notifications;
    public bool HasNotifications => _notifications.Count > 0;

    public void AddNotification(Notification notification) => _notifications.Add(notification);

    public void AddNotifications(ValidationResult result) =>
        result
            .Errors.ToList()
            .ForEach(error =>
                AddNotification(Notification.Add(error.ErrorCode, error.ErrorMessage))
            );
}
