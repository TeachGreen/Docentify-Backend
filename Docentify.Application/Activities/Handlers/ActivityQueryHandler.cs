using Docentify.Application.Activities.Queries;
using Docentify.Application.Activities.ValueObjects;
using Docentify.Application.Activities.ViewModels;
using Docentify.Application.Utils;
using Docentify.Domain.Exceptions;
using Docentify.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Docentify.Application.Activities.Handlers;

public class ActivityQueryHandler(DatabaseContext context, IConfiguration configuration)
{
    public async Task<ActivityViewModel> GetActivityByIdUserAsync(GetActivityByIdQuery query, HttpRequest request,  CancellationToken cancellationToken)
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
        
        var activity = await context.Activities.AsNoTracking()
            .Where(a => a.Id == query.ActivityId)
            .Include(a => a.Step)
            .Include(a => a.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(cancellationToken);
        if (activity is null)
        {
            throw new NotFoundException("No activity with the provided id was found");
        }
        
        if (!user.Enrollments.Select(e => e.CourseId).Contains(activity.Step.CourseId))
        {
            throw new ForbiddenException("User is not enrolled in the course that contains the provided activity");
        }
    
        return new ActivityViewModel
        {
            Id = activity.Id,
            AllowedAttempts = activity.AllowedAttempts,
            StepId = activity.StepId,
            Questions = activity.Questions.Select(q => new QuestionValueObject
            {
                Id = q.Id,
                Statement = q.Statement,
                Options = q.Options.Select(o => new OptionValueObject
                {
                    Id = o.Id,
                    Text = o.Text,
                    IsCorrect = null
                }).ToList()
            }).ToList()
        };
    }
    
    public async Task<ActivityViewModel> GetActivityByStepIdUserAsync(GetActivityByStepIdQuery query, HttpRequest request,  CancellationToken cancellationToken)
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
        
        var activity = await context.Activities.AsNoTracking()
            .Where(a => a.StepId == query.StepId)
            .Include(a => a.Step)
            .Include(a => a.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(cancellationToken);
        if (activity is null)
        {
            throw new NotFoundException("No activity associated with the step provided was found");
        }
        
        if (!user.Enrollments.Select(e => e.CourseId).Contains(activity.Step.CourseId))
        {
            throw new ForbiddenException("User is not enrolled in the course that contains the provided activity");
        }
    
        return new ActivityViewModel
        {
            Id = activity.Id,
            AllowedAttempts = activity.AllowedAttempts,
            StepId = activity.StepId,
            Questions = activity.Questions.Select(q => new QuestionValueObject
            {
                Id = q.Id,
                Statement = q.Statement,
                Options = q.Options.Select(o => new OptionValueObject
                {
                    Id = o.Id,
                    Text = o.Text,
                    IsCorrect = null
                }).ToList()
            }).ToList()
        };
    }
    
    public async Task<ActivityViewModel> GetActivityByIdInstitutionAsync(GetActivityByIdQuery query, HttpRequest request,  CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        
        var institution = await context.Institutions.AsNoTracking()
            .Where(u => u.Email == jwtData["email"])
            .Include(i => i.Courses)
            .FirstOrDefaultAsync(cancellationToken);
        if (institution is null)
        {
            throw new NotFoundException("No institution with the provided authentication was found");
        }
        
        var activity = await context.Activities.AsNoTracking()
            .Where(a => a.Id == query.ActivityId)
            .Include(a => a.Step)
            .Include(a => a.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(cancellationToken);
        if (activity is null)
        {
            throw new NotFoundException("No activity with the provided id was found");
        }
        
        if (!institution.Courses.Select(e => e.Id).Contains(activity.Step.CourseId))
        {
            throw new ForbiddenException("Institution is not owner of the course that contains the provided step");
        }
    
        return new ActivityViewModel
        {
            Id = activity.Id,
            AllowedAttempts = activity.AllowedAttempts,
            StepId = activity.StepId,
            Questions = activity.Questions.Select(q => new QuestionValueObject
            {
                Id = q.Id,
                Statement = q.Statement,
                Options = q.Options.Select(o => new OptionValueObject
                {
                    Id = o.Id,
                    Text = o.Text,
                    IsCorrect = o.IsCorrect
                }).ToList()
            }).ToList()
        };
    }
    
    public async Task<List<AttemptViewModel>> GetActivityAttemptHistoryUserAsync(GetActivityAttemptHistoryQuery query, HttpRequest request,  CancellationToken cancellationToken)
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
        
        var activity = await context.Activities.AsNoTracking()
            .Where(a => a.Id == query.ActivityId)
            .Include(a => a.Step)
            .Include(a => a.Questions)
            .ThenInclude(q => q.Options)
            .Include(a => a.Attempts)
            .FirstOrDefaultAsync(cancellationToken);
        if (activity is null)
        {
            throw new NotFoundException("No activity with the provided id was found");
        }
        
        if (!user.Enrollments.Select(e => e.CourseId).Contains(activity.Step.CourseId))
        {
            throw new ForbiddenException("User is not enrolled in the course that contains the provided activity");
        }

        var attempts = activity.Attempts.Where(a => a.UserId == user.Id).Select(a => new AttemptViewModel
        {
            UserId = a.UserId,
            ActivityId = a.ActivityId,
            Score = a.Score,
            Date = a.Date,
            Passed = a.Passed
        }).ToList();
        
        return attempts;
    }
    
    public async Task<List<AttemptViewModel>> GetActivityAttemptHistoryInstitutionAsync(GetActivityAttemptHistoryQuery query, HttpRequest request,  CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        var institution = await context.Institutions
            .Include(i => i.Courses)
            .ThenInclude(c => c.Steps)
            .ThenInclude(s => s.Activity)
            .ThenInclude(a => a.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(i => i.Email == jwtData["email"], cancellationToken);
        if (institution is null)
        {
            throw new NotFoundException("No institution with the provided authentication was found");
        }
        
        var activity = await context.Activities.AsNoTracking()
            .Where(a => a.Id == query.ActivityId)
            .Include(a => a.Step)
            .Include(a => a.Questions)
            .ThenInclude(q => q.Options)
            .Include(a => a.Attempts)
            .FirstOrDefaultAsync(cancellationToken);
        if (!institution.Courses.Select(c => c.Id).Contains(activity.Step.CourseId))
        {
            throw new NotFoundException("No course containing an activity with the provided id was found in your institution");
        }

        return activity.Attempts.Select(a => new AttemptViewModel
        {
            UserId = a.UserId,
            ActivityId = a.ActivityId,
            Score = a.Score,
            Date = a.Date,
            Passed = a.Score >= activity.Questions.Count / 2
        }).ToList();
    }
}