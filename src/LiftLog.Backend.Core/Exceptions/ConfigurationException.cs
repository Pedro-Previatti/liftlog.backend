namespace LiftLog.Backend.Core.Exceptions;

public class ConfigurationException : Exception
{
    public IDictionary<string, string[]> ValidationErrors { get; } =
        new Dictionary<string, string[]>();

    public bool HasValidationErrors => ValidationErrors.Count > 0;

    private ConfigurationException(string message)
        : base(message) { }

    private ConfigurationException(string message, Exception innerException)
        : base(message, innerException) { }

    private ConfigurationException(string message, IDictionary<string, string[]>? validationErrors)
        : base(message)
    {
        if (validationErrors is null)
            return;

        foreach (var error in validationErrors)
        {
            ValidationErrors[error.Key] = error.Value;
        }
    }

    public static ConfigurationException MissingKey(string configKey) =>
        new($"Mandatory configuration '{configKey}' not found");

    public static ConfigurationException InvalidValue(string configKey, string reason) =>
        new($"Invalid value for configuration '{configKey}': {reason}");

    public static ConfigurationException ValidationFailed(
        string message,
        IDictionary<string, string[]> validationErrors
    ) => new(message, validationErrors);

    public static ConfigurationException FromFluentValidation(
        string message,
        IEnumerable<FluentValidation.Results.ValidationFailure> validationFailures
    )
    {
        var validationFailuresArray = validationFailures.ToArray();

        var errors = validationFailuresArray
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => string.IsNullOrEmpty(g.Key) ? "General" : g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        return new ConfigurationException(message, errors);
    }
}
