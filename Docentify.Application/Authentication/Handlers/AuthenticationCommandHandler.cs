using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Docentify.Application.Authentication.Commands;
using Docentify.Domain.Common.Enums;
using Docentify.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Docentify.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;
using JwtSecurityTokenHandler = System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler;

namespace Docentify.Application.Authentication.Handlers;

public class AuthenticationCommandHandler(DatabaseContext context, IConfiguration configuration) 
{
    private (string, string)  CreatePasswordHash(string password)
    {
        var rng = RandomNumberGenerator.Create();
        var salt = new byte[32];
        rng.GetBytes(salt);
        rng.Dispose();
        
        var passwordBytes = Encoding.UTF8.GetBytes(password + configuration["PasswordPepper"]);
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

    public async Task<UserEntity> RegisterUserAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var parsedPasswordHash = CreatePasswordHash(command.Password);
        var passwordHash = new UserPasswordHashEntity {HashedPassword = parsedPasswordHash.Item1, Salt = parsedPasswordHash.Item2};

        var user = new UserEntity
        {
            Name = command.Name,
            BirthDate = command.BirthDate,
            Email = command.Email,
            Telephone = command.Telephone,
            Document = command.Document,
            Gender = command.Gender
        };
        
        await context.Users.AddAsync(user, cancellationToken);
        passwordHash.User = user;
        await context.UserPasswordHashes.AddAsync(passwordHash, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }
    
    public async Task<(EStatusCode, string)> LoginUserAsync(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var parsedPasswordHash = CreatePasswordHash(command.Password);
        
        var user = context.Users.AsNoTracking()
            .Include(u => u.UserPasswordHash)
            .FirstOrDefault(u => u.Email == command.Email);
        
        if (user.UserPasswordHash.HashedPassword != parsedPasswordHash.Item1)
        {
            return (EStatusCode.Unauthorized, "Invalid user credentials");
        }
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Name)
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(configuration["Jwt:Issuer"],
            "User",
            claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials);

        var jwtHandler = new JwtSecurityTokenHandler();

        var jwtString = jwtHandler.WriteToken(jwt);
        
        return (EStatusCode.Ok, jwtString);
    }
}