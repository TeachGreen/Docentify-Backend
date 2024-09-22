using System.Security.Cryptography;
using System.Text;
using Docentify.Application.Users.Commands;
using Docentify.Domain.Entities;
using Docentify.Infrastructure.Database;
using Microsoft.Extensions.Configuration;

namespace Docentify.Application.Users.Handlers;

public class UserCommandHandler(DatabaseContext context, IConfiguration configuration) 
{
    public UserPasswordHashEntity CreatePasswordHash(string password)
    {
        var rng = RandomNumberGenerator.Create();
        var salt = new byte[32];
        rng.GetBytes(salt);

        var sha256 = SHA256.Create();
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password + configuration["PasswordPepper"]);
        byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

        Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
        Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

        byte[] hashedBytes = sha256.ComputeHash(saltedPassword);

        byte[] hashedPasswordWithSalt = new byte[hashedBytes.Length + salt.Length];
        Buffer.BlockCopy(salt, 0, hashedPasswordWithSalt, 0, salt.Length);
        Buffer.BlockCopy(hashedBytes, 0, hashedPasswordWithSalt, salt.Length, hashedBytes.Length);

        var passwordHash = Convert.ToBase64String(hashedPasswordWithSalt);
        return new UserPasswordHashEntity { HashedPassword = passwordHash, Salt = Convert.ToBase64String(salt) };
    }
    
    public async Task<UserEntity> RegisterUserAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var passwordHash = CreatePasswordHash(command.Password);

        var user = new UserEntity
        {
            Name = command.Name,
            BirthDate = command.BirthDate,
            Email = command.Email,
            Telephone = command.Telephone,
            Document = command.Document
        };
        
        await context.Users.AddAsync(user, cancellationToken);
        passwordHash.User = user;
        await context.PasswordHashes.AddAsync(passwordHash, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }
}