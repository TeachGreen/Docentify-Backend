using System.Security.Cryptography;
using System.Text;

namespace Docentify.Application.Utils;

public static class AuthenticationUtils
{
    public static (string, string) CreatePasswordHash(string password, string pepper)
    {
        var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[32];
        rng.GetBytes(saltBytes);
        rng.Dispose();

        var salt = Convert.ToBase64String(saltBytes);
        
        var saltedPassword = password + salt + pepper;
        
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(saltedPassword));

        var passwordHash = Convert.ToBase64String(hashedBytes);
        return (passwordHash, salt);
    }
    
    public static string RecreatePasswordHash(string password, string salt, string pepper)
    {
        var saltedPassword = password + salt + pepper;
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(saltedPassword));

        var passwordHash = Convert.ToBase64String(hashedBytes);
        return passwordHash;
    }
}