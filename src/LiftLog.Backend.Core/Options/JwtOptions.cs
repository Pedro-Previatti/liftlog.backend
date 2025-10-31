using System.ComponentModel.DataAnnotations;

namespace LiftLog.Backend.Core.Options;

public class JwtOptions
{
    [Required(ErrorMessage = "Jwt:Key is required")]
    [MinLength(32, ErrorMessage = "Jwt:Key must be at least 32 characters long for security")]
    public required string Key { get; init; }

    [Required(ErrorMessage = "Jwt:Issuer is required")]
    public required string Issuer { get; init; }

    [Required(ErrorMessage = "Jwt:Audience is required")]
    public required string Audience { get; init; }

    [Required(ErrorMessage = "Jwt:ExpirationDays is required")]
    [Range(1, 30, ErrorMessage = "Jwt:ExpirationDays must be between 1 and 30.")]
    public required int ExpirationDays { get; init; }
}
