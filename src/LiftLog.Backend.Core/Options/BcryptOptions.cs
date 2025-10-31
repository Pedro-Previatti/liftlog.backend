using System.ComponentModel.DataAnnotations;

namespace LiftLog.Backend.Core.Options;

public class BcryptOptions
{
    [Required(ErrorMessage = "Bcrypt:WorkFactor is required")]
    [Range(4, 32, ErrorMessage = "Bcrypt:WorkFactor must be between 4 and 32")]
    public required int WorkFactor { get; init; }
}
