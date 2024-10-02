using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Docentify.Application.Authentication.Commands;
using Docentify.Application.Utils;
using Docentify.Domain.Entities;
using Docentify.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Docentify.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;
using JwtSecurityTokenHandler = System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler;

namespace Docentify.Application.Authentication.Handlers;

public class AuthenticationCommandHandler(DatabaseContext context, IConfiguration configuration)
{
    public async Task<UserEntity> RegisterUserAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var parsedPasswordHash =
            AuthenticationUtils.CreatePasswordHash(command.Password!, configuration["PasswordPepper"]!);
        var passwordHash = new UserPasswordHashEntity
            { HashedPassword = parsedPasswordHash.Item1, Salt = parsedPasswordHash.Item2 };

        if (await context.Users.Where(user => user.Email == command.Email)
                .SingleOrDefaultAsync(cancellationToken) is not null)
            throw new ConflictException("An user with that e-mail already exists");

        var user = new UserEntity
        {
            Name = command.Name!,
            BirthDate = command.BirthDate,
            Email = command.Email!,
            Telephone = command.Telephone,
            Document = command.Document!,
            Gender = command.Gender
        };

        await context.Users.AddAsync(user, cancellationToken);
        passwordHash.User = user;
        await context.UserPasswordHashes.AddAsync(passwordHash, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<string> LoginUserAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var parsedPasswordHash =
            AuthenticationUtils.CreatePasswordHash(command.Password!, configuration["PasswordPepper"]!);

        var user = await context.Users.AsNoTracking()
            .Include(u => u.UserPasswordHash)
            .FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (user is null)
            throw new NotFoundException("Invalid user credentials");
        if (user.UserPasswordHash.HashedPassword != parsedPasswordHash.Item1)
            throw new UnauthorizedException("Invalid user credentials");

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Name)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(configuration["Jwt:Issuer"],
            "User",
            claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials);

        var jwtHandler = new JwtSecurityTokenHandler();

        var jwtString = jwtHandler.WriteToken(jwt);

        return jwtString;
    }

    public async Task<InstitutionEntity> RegisterInstitutionAsync(RegisterInstitutionCommand command,
        CancellationToken cancellationToken)
    {
        var parsedPasswordHash =
            AuthenticationUtils.CreatePasswordHash(command.Password!, configuration["PasswordPepper"]!);
        var passwordHash = new InstitutionPasswordHashEntity
            { HashedPassword = parsedPasswordHash.Item1, Salt = parsedPasswordHash.Item2 };
        
        if (await context.Institutions.Where(institution => institution.Email == command.Email)
                .SingleOrDefaultAsync(cancellationToken) is not null)
            throw new ConflictException("An institution with that e-mail already exists");

        var institution = new InstitutionEntity
        {
            Name = command.Name!,
            Email = command.Email!,
            Telephone = command.Telephone,
        };

        await context.Institutions.AddAsync(institution, cancellationToken);
        passwordHash.Institution = institution;
        await context.InstitutionPasswordHashes.AddAsync(passwordHash, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
        return institution;
    }

    public async Task<string> LoginInstitutionAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var parsedPasswordHash =
            AuthenticationUtils.CreatePasswordHash(command.Password!, configuration["PasswordPepper"]!);

        var institution = await context.Institutions.AsNoTracking()
            .Include(u => u.InstitutionPasswordHash)
            .FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (institution is null)
            throw new NotFoundException("Invalid institution credentials");
        if (institution.InstitutionPasswordHash.HashedPassword != parsedPasswordHash.Item1)
            throw new UnauthorizedException("Invalid institution credentials");
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, institution.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, institution.Name)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(configuration["Jwt:Issuer"],
            "Institution",
            claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials);

        var jwtHandler = new JwtSecurityTokenHandler();

        var jwtString = jwtHandler.WriteToken(jwt);

        return jwtString;
    }
}