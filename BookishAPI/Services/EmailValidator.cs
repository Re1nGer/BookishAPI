using System.Text.RegularExpressions;

namespace BookishAPI;

public static class EmailValidator
{
    private static readonly Regex EmailRegex = new (
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static bool IsValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email);
    }
}
