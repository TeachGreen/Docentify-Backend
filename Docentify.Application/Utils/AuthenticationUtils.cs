using System.Security.Cryptography;
using System.Text;

namespace Docentify.Application.Utils;

public static class AuthenticationUtils
{
    public static (string, string) CreatePasswordHash(string password, string pepper)
    {
        var rng = RandomNumberGenerator.Create();
        var salt = new byte[32];
        rng.GetBytes(salt);
        rng.Dispose();
        
        var passwordBytes = Encoding.UTF8.GetBytes(password + pepper);
        var saltedPassword = new byte[passwordBytes.Length + salt.Length];

        Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
        Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

        var hashedBytes = SHA256.HashData(saltedPassword);

        var hashedPasswordWithSalt = new byte[hashedBytes.Length + salt.Length];
        Buffer.BlockCopy(salt, 0, hashedPasswordWithSalt, 0, salt.Length);
        Buffer.BlockCopy(hashedBytes, 0, hashedPasswordWithSalt, salt.Length, hashedBytes.Length);

        var passwordHash = Convert.ToBase64String(hashedPasswordWithSalt);
        return (passwordHash, Convert.ToBase64String(salt));
    }
}