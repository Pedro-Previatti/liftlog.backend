using System.Net.Mail;
using LiftLog.Backend.Core.Shared;

namespace LiftLog.Backend.Core.Helpers;

public class StringHelpers
{
    public static bool IsValidCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        if (!RegexPatterns.CpfPattern().IsMatch(cpf))
            return false;

        var numbers = RegexPatterns.OnlyNumbersPattern().Replace(cpf, "");
        if (numbers.Length != 11)
            return false;

        if (numbers.Distinct().Count() == 1)
            return false;

        var digits = numbers.Select(c => c - '0').ToArray();

        var sum = 0;
        for (var i = 0; i < 9; i++)
            sum += digits[i] * (10 - i);

        var r = sum % 11;
        var d10 = r < 2 ? 0 : 11 - r;
        if (digits[9] != d10)
            return false;

        sum = 0;
        for (var i = 0; i < 10; i++)
            sum += digits[i] * (11 - i);

        r = sum % 11;
        var d11 = r < 2 ? 0 : 11 - r;
        return digits[10] == d11;
    }

    public static bool IsValidPhoneNumber(string number) =>
        !string.IsNullOrWhiteSpace(number) && RegexPatterns.PhoneNumberPattern().IsMatch(number);

    public static bool IsValidEmail(string email)
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

    public static bool IsCryptographedPassword(string password) =>
        !string.IsNullOrWhiteSpace(password) && RegexPatterns.BcryptPattern().IsMatch(password);
}
