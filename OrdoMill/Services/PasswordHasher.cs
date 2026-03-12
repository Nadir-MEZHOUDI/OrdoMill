using System.Security.Cryptography;

namespace OrdoMill.Services;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 10000;
    private static readonly HashAlgorithmName HashAlgorithmName = HashAlgorithmName.SHA256;
    private const char Delimiter = ';';

    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password));

        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName, HashSize);

        return string.Join(Delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        if (string.IsNullOrEmpty(hashedPassword))
            return false;

        var elements = hashedPassword.Split(Delimiter);
        if (elements.Length != 2)
        {
            return IsLegacyPasswordMatch(password, hashedPassword);
        }

        var salt = Convert.FromBase64String(elements[0]);
        var hash = Convert.FromBase64String(elements[1]);

        var newHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName, HashSize);

        return CryptographicOperations.FixedTimeEquals(hash, newHash);
    }

    private static bool IsLegacyPasswordMatch(string password, string hashedPassword)
    {
        return password == hashedPassword;
    }

    public static bool NeedsRehash(string hashedPassword)
    {
        if (string.IsNullOrEmpty(hashedPassword))
            return true;

        var elements = hashedPassword.Split(Delimiter);
        return elements.Length != 2;
    }
}
