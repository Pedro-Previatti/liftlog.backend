using System.Text.RegularExpressions;

namespace LiftLog.Backend.Core.Shared;

public static partial class RegexPatterns
{
    [GeneratedRegex(
        @"^\$2[aby]\$[0-9]{2}\$[./A-Za-z0-9]{53}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    )]
    public static partial Regex BcryptPattern();

    [GeneratedRegex(
        @"^\d{3}\.\d{3}\.\d{3}-\d{2}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    )]
    public static partial Regex CpfPattern();

    [GeneratedRegex(
        @"^\+\d{1,3} \(\d{2}\) \d{5}-\d{4}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    )]
    public static partial Regex PhoneNumberPattern();

    [GeneratedRegex(@"^[\p{L}\p{M}'\-\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    public static partial Regex OnlyLettersPattern();

    [GeneratedRegex(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>/?]).{12,}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    )]
    public static partial Regex PasswordPattern();
}
