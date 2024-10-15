using AutoMapper;
using Docentify.Application.Institutions.Commands;
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
        var institution = await context.Institutions
            .FirstOrDefaultAsync(i => i.Email == jwtData["email"], cancellationToken);

        if (institution is null)
        {
            throw new NotFoundException("No institution with the provided authentication was found");
        }
        
        var mapper = new MapperConfiguration(cfg => 
            cfg.CreateMap<UpdateInstitutionCommand, InstitutionEntity>()
                .ForAllMembers(opts => 
                    opts.Condition((src, dest, srcMember) => srcMember != null))
        ).CreateMapper();
        mapper.Map(command, institution);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            throw new ConflictException("The changes cannot be saved due to the uniqueness requirements");
        }
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
        var institution = await context.Institutions
            .Include(i => i.Users)
            .FirstOrDefaultAsync(i => i.Email == jwtData["email"], cancellationToken);

        if (institution is null)
        {
            throw new NotFoundException("No institution with the provided authentication was found");
        }
        
        if (institution.Users.Any(u => u.Id == command.UserId))
        {
            throw new ConflictException("The user with the provided id is already associated with the institution");
        }

        var user = await context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("No user with the provided id was found");
        }
        
        institution.Users.Add(user);
        
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task AssociateUserByDocumentAsync(AssociateUserByDocumentCommand command, HttpRequest request, CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        var institution = await context.Institutions
            .Include(i => i.Users)
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
        
        if (institution.Users.Any(u => u.Document == command.Document))
        {
            throw new ConflictException("The user with the provided id is already associated with the institution");
        }
        
        institution.Users.Add(user);
        
        await context.SaveChangesAsync(cancellationToken);
    }
}