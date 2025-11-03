using FluentValidation;
using FluentValidation.Results;
using LiftLog.Backend.Core.Helpers;

namespace LiftLog.Backend.Application.Validators;

public class RequestValidator
{
    public readonly NotificationContext NotificationContext = new();

    public ValidationResult? ValidationResult { get; private set; }

    public bool Valid { get; private set; }
    public bool Invalid => !Valid;

    public bool Validate<TModel>(TModel model, AbstractValidator<TModel> validator)
    {
        ValidationResult = validator.Validate(model);
        Valid = ValidationResult.IsValid;

        if (Invalid)
            NotificationContext.AddNotifications(ValidationResult);

        return Valid;
    }
}
