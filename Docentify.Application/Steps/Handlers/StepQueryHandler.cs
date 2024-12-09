using Docentify.Application.Steps.Queries;
using Docentify.Application.Steps.ViewModels;
using Docentify.Application.Utils;
using Docentify.Domain.Exceptions;
using Docentify.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Docentify.Application.Steps.Handlers;

public class StepQueryHandler(DatabaseContext context, IConfiguration configuration)
{
    public async Task<StepViewModel> GetStepByIdUserAsync(GetStepByIdQuery query, HttpRequest request,  CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        
        var user = await context.Users.AsNoTracking()
            .Include(u => u.Enrollments)
            .Where(u => u.Email == jwtData["email"])
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("No user with the provided authentication was found");
        }
        
        var step = await context.Steps.AsNoTracking()
            .Include(s => s.UserProgresses)
            .ThenInclude(u => u.Enrollment)
            .Include((s => s.Activity))
            .Where(s => s.Id == query.StepId)
            .FirstOrDefaultAsync(cancellationToken);
        if (step is null)
        {
            throw new NotFoundException("No step with the provided id was found");
        }
        
        if (!user.Enrollments.Select(e => e.CourseId).Contains(step.CourseId))
        {
            throw new ForbiddenException("User is not enrolled in the course that contains the provided step");
        }

        return new StepViewModel
        {
            Id = step.Id,
            Title = step.Title,
            Description = step.Description,
            Order = step.Order,
            Type = step.Type,
            Content = step.Content,
            IsCompleted = step.UserProgresses
                .FirstOrDefault(p => p.Enrollment.UserId == user.Id) is not null
        };
    }
    
    public async Task<StepViewModel> GetStepByIdInstitutionAsync(GetStepByIdQuery query, HttpRequest request,  CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        
        var institution = await context.Institutions.AsNoTracking()
            .Where(u => u.Email == jwtData["email"])
            .FirstOrDefaultAsync(cancellationToken);
        if (institution is null)
        {
            throw new NotFoundException("No institution with the provided authentication was found");
        }
        
        var step = await context.Steps.AsNoTracking()
            .Where(s => s.Id == query.StepId)
            .FirstOrDefaultAsync(cancellationToken);
        if (step is null)
        {
            throw new NotFoundException("No step with the provided id was found");
        }
        
        if (!institution.Courses.Select(e => e.Id).Contains(step.CourseId))
        {
            throw new ForbiddenException("Institution is not owner of the course that contains the provided step");
        }

        return new StepViewModel
        {
            Id = step.Id,
            Title = step.Title,
            Description = step.Description,
            Order = step.Order,
            Type = step.Type,
            Content = step.Content
        };
    }

}