using System.Net.Mail;
using System.Text.RegularExpressions;
using LiftLog.Backend.Core.Shared;

namespace LiftLog.Backend.Core.Helpers;

public static class StringHelpers
{
    private static readonly string[] SqlKeywords =
    [
        "--",
        "/*",
        "*/",
        ";",
        "SELECT",
        "INSERT",
        "UPDATE",
        "DELETE",
        "DROP",
        "UNION",
        "EXEC",
        "EXECUTE",
        "TRUNCATE",
        "CREATE",
        "ALTER",
        "GRANT",
        "REVOKE",
        "MERGE",
        "CALL",
    ];

    public static bool IsValidCpf(this string cpf)
    {
        var clean = new string(cpf.Where(char.IsDigit).ToArray());

        if (clean.Length != 11 || clean.Distinct().Count() == 1)
            return false;

        var sum = 0;
        for (var i = 0; i < 9; i++)
            sum += (clean[i] - '0') * (10 - i);

        var remainder = sum % 11;
        var first = remainder < 2 ? 0 : 11 - remainder;

        sum = 0;
        for (var i = 0; i < 10; i++)
            sum += (clean[i] - '0') * (11 - i);

        remainder = sum % 11;
        var second = remainder < 2 ? 0 : 11 - remainder;

        return clean[9] - '0' == first && clean[10] - '0' == second;
    }

    public static bool IsValidPhoneNumber(this string number) =>
        !string.IsNullOrWhiteSpace(number) && RegexPatterns.PhoneNumberPattern().IsMatch(number);

    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        try
        {
            var addr = new MailAddress(email);
            return string.Equals(addr.Address, email, StringComparison.Ordinal);
        }
        catch
        {
            return false;
        }
    }

    public static bool IsCryptographedPassword(this string password) =>
        !string.IsNullOrWhiteSpace(password) && RegexPatterns.BcryptPattern().IsMatch(password);

    public static bool IsSafeForSqlInput(this string? input)
    {
        if (string.IsNullOrEmpty(input))
            return true;

        var upperInput = input.ToUpper();

        foreach (var keyword in SqlKeywords)
        {
            var pattern = @"\b" + Regex.Escape(keyword) + @"\b";
            if (Regex.IsMatch(upperInput, pattern))
            {
                return false;
            }
        }

        var singleQuoteCount = CountOccurrences(upperInput, '\'');
        var doubleQuoteCount = CountOccurrences(upperInput, '"');

        if (singleQuoteCount > 5 || doubleQuoteCount > 5)
            return false;

        var semicolonCount = CountOccurrences(upperInput, ';');

        return !(semicolonCount > 2);
    }

    private static int CountOccurrences(this string text, char character)
    {
        var count = 0;

        foreach (var c in text)
            if (c == character)
                count++;

        return count;
    }
}
