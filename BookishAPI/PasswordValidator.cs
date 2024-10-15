namespace BookishAPI;

public static class PasswordValidator
{
    private const int MinimumLength = 8;

    public static bool IsValid(string password)
    {
        return !string.IsNullOrWhiteSpace(password) && password.Length >= MinimumLength;
    }

    public static string Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return "Password cannot be empty";
        }

        if (password.Length < MinimumLength)
        {
            return $"Password must be at least {MinimumLength} characters long";
        }

        return string.Empty; // No error
    }
}
