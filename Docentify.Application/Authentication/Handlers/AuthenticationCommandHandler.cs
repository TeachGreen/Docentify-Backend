using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Docentify.Application.Authentication.Commands;
using Docentify.Application.Authentication.ViewModels;
using Docentify.Application.Utils;
using Docentify.Domain.Entities;
using Docentify.Domain.Entities.User;
using Docentify.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Docentify.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;
using JwtSecurityTokenHandler = System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler;
using RegisterUserCommand = Docentify.Application.Authentication.Commands.RegisterUserCommand;

namespace Docentify.Application.Authentication.Handlers;

public class AuthenticationCommandHandler(DatabaseContext context, IConfiguration configuration)
{
    public async Task<RegisterUserViewModel> RegisterUserAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var parsedPasswordHash =
            AuthenticationUtils.CreatePasswordHash(command.Password!, configuration["PasswordPepper"]!);
        var passwordHash = new UserPasswordHashEntity
            { HashedPassword = parsedPasswordHash.Item1, Salt = parsedPasswordHash.Item2 };

        if (await context.Users.Where(user => user.Email == command.Email)
                .SingleOrDefaultAsync(cancellationToken) is not null)
            throw new ConflictException("An user with that e-mail is already registered");
        if (await context.Users.Where(user => user.Document == command.Document)
                .SingleOrDefaultAsync(cancellationToken) is not null)
            throw new ConflictException("An user with that document is already registered");

        var user = new UserEntity
        {
            Name = command.Name!,
            BirthDate = command.BirthDate,
            Email = command.Email!,
            Telephone = command.Telephone,
            Document = command.Document!,
            Gender = command.Gender,
            UserPasswordHash = passwordHash
        };
        await context.Users.AddAsync(user, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);
        return new RegisterUserViewModel
        {
            Name = user.Name, 
            BirthDate = user.BirthDate, 
            Email = user.Email, 
            Gender = user.Gender,
            Telephone = user.Telephone,
            Document = user.Document
        };
    }

    public async Task<LoginViewModel> LoginUserAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await context.Users.AsNoTracking()
            .Include(u => u.UserPasswordHash)
            .FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (user is null)
            throw new NotFoundException("No user with that e-mail was found");
        
        var parsedPasswordHash =
            AuthenticationUtils.RecreatePasswordHash(command.Password!, user.UserPasswordHash.Salt, configuration["PasswordPepper"]!);
        if (user.UserPasswordHash.HashedPassword != parsedPasswordHash)
            throw new UnauthorizedException("Invalid user credentials");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.UniqueName, user.Name),
            new(JwtRegisteredClaimNames.Aud, "Users"),
            new(ClaimTypes.Role, "Users")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(configuration["Jwt:Issuer"],
            "Users",
            claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials);

        var jwtHandler = new JwtSecurityTokenHandler();

        var jwtString = jwtHandler.WriteToken(jwt);

        return new LoginViewModel
        {
            Jwt = jwtString
        };
    }

    public async Task<RegisterInstitutionViewModel> RegisterInstitutionAsync(RegisterInstitutionCommand command,
        CancellationToken cancellationToken)
    {
        var parsedPasswordHash =
            AuthenticationUtils.CreatePasswordHash(command.Password!, configuration["PasswordPepper"]!);
        var passwordHash = new InstitutionPasswordHashEntity
            { HashedPassword = parsedPasswordHash.Item1, Salt = parsedPasswordHash.Item2 };
        
        if (await context.Institutions.Where(institution => institution.Email == command.Email)
                .SingleOrDefaultAsync(cancellationToken) is not null)
            throw new ConflictException("An institution with that e-mail is already registered");
        if (await context.Institutions.Where(institution => institution.Name == command.Name)
                .SingleOrDefaultAsync(cancellationToken) is not null)
            throw new ConflictException("An institution with that name is already registered");

        var institution = new InstitutionEntity
        {
            Name = command.Name!,
            Email = command.Email!,
            Telephone = command.Telephone,
            Document = command.Document!,
            Address = command.Address,
            InstitutionPasswordHash = passwordHash
        };
        await context.Institutions.AddAsync(institution, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);
        return new RegisterInstitutionViewModel
        {
            Name = institution.Name, 
            Email = institution.Email, 
            Telephone = institution.Telephone,
            Document = institution.Document,
            Address = institution.Address,
        };
    }

    public async Task<LoginViewModel> LoginInstitutionAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var institution = await context.Institutions.AsNoTracking()
            .Include(u => u.InstitutionPasswordHash)
            .FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (institution is null)
            throw new NotFoundException("No institution with that e-mail was found");
        
        var parsedPasswordHash =
            AuthenticationUtils.RecreatePasswordHash(command.Password!, institution.InstitutionPasswordHash.Salt, configuration["PasswordPepper"]!);
        if (institution.InstitutionPasswordHash.HashedPassword != parsedPasswordHash)
            throw new UnauthorizedException("Invalid institution credentials");
        
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Email, institution.Email),
            new(JwtRegisteredClaimNames.UniqueName, institution.Name),
            new(JwtRegisteredClaimNames.Aud, "Institutions"),
            new(ClaimTypes.Role, "Institutions")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(configuration["Jwt:Issuer"],
            "Institutions",
            claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials);

        var jwtHandler = new JwtSecurityTokenHandler();

        var jwtString = jwtHandler.WriteToken(jwt);

        return new LoginViewModel
        {
            Jwt = jwtString
        };
    }
}