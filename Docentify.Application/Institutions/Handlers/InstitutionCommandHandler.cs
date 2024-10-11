using Docentify.Application.Institutions.Commands;
using Docentify.Application.Users.Commands;
using Docentify.Application.Users.ValueObject;
using Docentify.Application.Users.ViewModels;
using Docentify.Application.Utils;
using Docentify.Domain.Entities;
using Docentify.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Docentify.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Application.Institutions.Handlers;

public class InstitutionCommandHandler(DatabaseContext context, IConfiguration configuration)
{
    public async Task<InstitutionViewModel> UpdateInstitutionAsync(UpdateInstitutionCommand command, HttpRequest request, CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        var institution = await context.Institutions.AsNoTracking()
            .FirstOrDefaultAsync(i => i.Email == jwtData["email"], cancellationToken);

        if (institution is null)
        {
            throw new NotFoundException("No institution with the provided authentication was found");
        }
        
        if (command.Name is not null)
            institution.Name = command.Name;
        
        if (command.Email is not null)
            institution.Email = command.Email;
        
        if (command.Telephone is not null)
            institution.Telephone = command.Telephone;
        
        if (command.Document is not null)
            institution.Document = command.Document;

        if (command.Address is not null)
            institution.Address = command.Address;
        
        context.Institutions.Update(institution);
        
        await context.SaveChangesAsync(cancellationToken);
        return new InstitutionViewModel
        {
            Name = institution.Name, 
            Email = institution.Email, 
            Telephone = institution.Telephone,
            Document = institution.Document,
            Address = institution.Address
        };
    }
    
    public async Task AssociateUserByIdAsync(AssociateUserByIdCommand command, HttpRequest request, CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        var institution = await context.Institutions.AsNoTracking()
            .FirstOrDefaultAsync(i => i.Email == jwtData["email"], cancellationToken);

        if (institution is null)
        {
            throw new NotFoundException("No institution with the provided authentication was found");
        }

        var user = await context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
        
        if (user is null)
        {
            throw new NotFoundException("No user with the provided id was found");
        }
        
        institution.Users.Add(user);
        context.Institutions.Update(institution);
        
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task AssociateUserByDocumentAsync(AssociateUserByDocumentCommand command, HttpRequest request, CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        var institution = await context.Institutions.AsNoTracking()
            .FirstOrDefaultAsync(i => i.Email == jwtData["email"], cancellationToken);

        if (institution is null)
        {
            throw new NotFoundException("No institution with the provided authentication was found");
        }

        var user = await context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Document == command.Document, cancellationToken);
        
        if (user is null)
        {
            throw new NotFoundException("No user with the provided document was found");
        }
        
        institution.Users.Add(user);
        context.Institutions.Update(institution);
        
        await context.SaveChangesAsync(cancellationToken);
    }
}