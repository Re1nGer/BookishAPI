using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace BookishAPI;

public class PasswordHasher
{
    private const int SaltSize = 128 / 8; // 128 bits
    private const int HashSize = 256 / 8; // 256 bits
    private const int MinIterations = 100000; // Increased from 10,000
    private const int MaxIterations = 1000000; // Upper bound for iterations

    public static string HashPassword(string password)
    {
        // Generate a random salt
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Determine the number of iterations based on system performance
        int iterations = DetermineIterations();

        Console.WriteLine($"Iterations: {iterations}");

        // Hash the password
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: iterations,
            numBytesRequested: HashSize
        ));

        // Combine the salt, iterations, and hash
        return $"{Convert.ToBase64String(salt)}.{iterations}.{hashed}";
    }

    public static bool VerifyPassword(string storedHash, string providedPassword)
    {
        // Extract the salt, iterations, and hash from the stored value
        var parts = storedHash.Split('.');
        if (parts.Length != 3)
            return false;

        var salt = Convert.FromBase64String(parts[0]);
        var iterations = int.Parse(parts[1]);
        var hash = parts[2];

        // Hash the provided password with the extracted salt and iterations
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: providedPassword,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: iterations,
            numBytesRequested: HashSize
        ));

        // Compare the computed hash with the stored hash
        return hash == hashed;
    }

    private static int DetermineIterations()
    {
        // Measure system performance to determine the number of iterations
        const int testIterations = 10000;
        byte[] testSalt = new byte[SaltSize];
        string testPassword = "performance_test_password";

        var sw = System.Diagnostics.Stopwatch.StartNew();
        KeyDerivation.Pbkdf2(testPassword, testSalt, KeyDerivationPrf.HMACSHA256, testIterations, HashSize);
        sw.Stop();

        // Aim for ~500ms hashing time
        int targetMilliseconds = 500;
        int estimatedIterations = (int)((double)testIterations / sw.ElapsedMilliseconds * targetMilliseconds);

        return Math.Clamp(estimatedIterations, MinIterations, MaxIterations);
    }
}
