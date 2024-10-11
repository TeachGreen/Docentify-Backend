using Docentify.Application.Institutions.Queries;
using Docentify.Application.Institutions.ValueObject;
using Docentify.Application.Institutions.ViewModels;
using Docentify.Application.Users.ViewModels;
using Docentify.Application.Utils;
using Docentify.Domain.Exceptions;
using Docentify.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Docentify.Application.Institutions.Handlers;

public class InstitutionQueryHandler(DatabaseContext context, IConfiguration configuration)
{
    public async Task<InstitutionViewModel> GetInstitutionByIdAsync(GetInstitutionByIdQuery query, HttpRequest request,  CancellationToken cancellationToken)
    {
        var institution = await context.Institutions.AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == query.InstitutionId, cancellationToken);

        if (institution is null)
        {
            throw new NotFoundException("No institution with that id was found");
        }

        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        if (jwtData["email"] == institution.Email)
        {
            return new InstitutionViewModel
            {
                Name = institution.Name,
                Email = institution.Email,
                Telephone = institution.Telephone,
                Document = institution.Document,
                Address = institution.Address
            };            
        }
            
        return new InstitutionViewModel
        {
            Name = institution.Name,
            Email = institution.Email,
            Telephone = institution.Telephone
        };
    }
    
    public async Task<InstitutionUsersViewModel> GetInstitutionUsersAsync(GetInstitutionUsersQuery query, HttpRequest request,  CancellationToken cancellationToken)
    {
        var institution = await context.Institutions.AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == query.InstitutionId, cancellationToken);

        if (institution is null)
        {
            throw new NotFoundException("No institution with that id was found");
        }
        
        var users = await context.Users.AsNoTracking()
            .Where(u => u.Institutions.Select(i => i.Id).Contains(query.InstitutionId))
            .Skip((query.Page - 1) * query.Amount)
            .Take(query.Amount)
            .Select(u => new UserValueObject
            {
                UserId = u.Id,
                Name = u.Name,
                Email = u.Email
            })
            .ToListAsync(cancellationToken);
            
        return new InstitutionUsersViewModel
        {
            Users = users
        };
    }
}