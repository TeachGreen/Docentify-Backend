using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Docentify.Application.Authentication.Commands;
using Docentify.Application.Authentication.ViewModels;
using Docentify.Application.Utils;
using Docentify.Domain.Entities;
using Docentify.Domain.Entities.Courses;
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
        
        var docentify = await context.Institutions
            .Include(i => i.Courses)
            .Include(i => i.Users)
            .FirstOrDefaultAsync(i => i.Email == "docentify@gmail.com", cancellationToken);
        user.Institutions.Add(docentify);
        user.UserScore = new UserScoreEntity { Score = 0 };
        await context.Users.AddAsync(user, cancellationToken);
        docentify.Users.Add(user);
        
        foreach (var requiredCourse in docentify.Courses.Where(c => c.IsRequired.GetValueOrDefault()))
        {
            var enrollment = new EnrollmentEntity
            {
                CourseId = requiredCourse.Id,
                UserId = user.Id
            };
            user.Enrollments.Add(enrollment);
        }
        
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

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(configuration["JwtIssuer"],
            "Users",
            claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials);

        var jwtHandler = new JwtSecurityTokenHandler();

        var jwtString = jwtHandler.WriteToken(jwt);

        return new LoginViewModel
        {
            Jwt = jwtString,
            UserId = user.Id
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

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(configuration["JwtIssuer"],
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
    
    public async Task<PasswordChangeRequestViewModel> PasswordChangeRequestUserAsync(PasswordChangeRequestUserCommand command, CancellationToken cancellationToken)
    {
        var user = await context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (user is null)
            throw new NotFoundException("No user with that e-mail was found");
        
        var generator = new Random();
        var code = generator.Next(0, 1000000).ToString("D6");

        var passwordChangeSession = new PasswordChangeRequestEntity
        {
            Email = command.Email,
            Code = code,
            ExpiresAt = DateTime.Now.AddMinutes(30)
        }; 
        await context.PasswordChangeRequests.AddAsync(passwordChangeSession, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);
        
        MailMessage mail = new MailMessage
        {
            From = new MailAddress(configuration["EmailUser"], "Docentify")
        };

        mail.To.Add(new MailAddress(command.Email));

        mail.Subject = "Código de Confirmação de Identidade - Docentify";
        mail.Body = $"Seu código de confirmação de identidade é: {code}";
        mail.Priority = MailPriority.High;

        using (SmtpClient smtp = new SmtpClient(configuration["EmailHost"], 587))
        {
            smtp.Credentials = new NetworkCredential(configuration["EmailUser"], configuration["EmailPassword"]);
            smtp.EnableSsl = true;
            await smtp.SendMailAsync(mail);
        }

        return new PasswordChangeRequestViewModel
        {
            Id = passwordChangeSession.Id
        };
    }
    
    public async Task<IdentityConfirmationViewModel> IdentityConfirmationUserAsync(IdentityConfirmationUserCommand command, CancellationToken cancellationToken)
    {
        var session = await context.PasswordChangeRequests.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == command.Id, cancellationToken);
        
        return new IdentityConfirmationViewModel()
        {
            Confirmed = session.Code == command.Code
        };
    }
    
    public async Task<NewPasswordCreationViewModel> NewPasswordCreationUserAsync(NewPasswordCreationUserCommand command, CancellationToken cancellationToken)
    {
        var session = await context.PasswordChangeRequests.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == command.Id, cancellationToken);

        if (session.Code == command.Code)
        {
            var parsedPasswordHash =
                AuthenticationUtils.CreatePasswordHash(command.Password!, configuration["PasswordPepper"]!);
            
            var user = await context.Users
                .Include(u => u.UserPasswordHash)
                .FirstOrDefaultAsync(u => u.Email == session.Email, cancellationToken);
            
            user.UserPasswordHash.HashedPassword = parsedPasswordHash.Item1;
            user.UserPasswordHash.Salt = parsedPasswordHash.Item2;

            await context.SaveChangesAsync();
        }
        
        return new NewPasswordCreationViewModel
        {
            Success = session.Code == command.Code
        };
    }

}