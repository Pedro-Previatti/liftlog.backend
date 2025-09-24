using FluentValidation;
using FluentValidation.Results;
using LiftLog.Backend.Core.Helpers;

namespace LiftLog.Backend.Core.Entities;

public class BaseEntity
{
    public required Guid Id { get; init; }
    public Guid CreatedBy { get; protected init; }
    public Guid UpdatedBy { get; protected set; }

    public required DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
    public required DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    private readonly NotificationContext _notificationContext = new();

    public ValidationResult? ValidationResult { get; private set; }

    public bool Valid { get; private set; }
    public bool Invalid => !Valid;

    protected bool Validate<TModel>(TModel model, AbstractValidator<TModel> validator)
    {
        ValidationResult = validator.Validate(model);
        Valid = ValidationResult.IsValid;

        if (Invalid)
            _notificationContext.AddNotifications(ValidationResult);

        return Valid;
    }

    public IReadOnlyList<Notification> GetNotifications() => _notificationContext.Notifications;
}
