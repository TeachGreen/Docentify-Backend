using Docentify.Application.Activities.Commands;
using Docentify.Application.Activities.ValueObjects;
using Docentify.Application.Activities.ViewModels;
using Docentify.Application.Utils;
using Docentify.Domain.Entities.Step;
using Docentify.Domain.Exceptions;
using Docentify.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Docentify.Application.Activities.Handlers;

public class ActivityCommandHandler(DatabaseContext context, IConfiguration configuration)
{
    public async Task<ActivityViewModel> InsertActivityAsync(InsertActivityCommand command, HttpRequest request, CancellationToken cancellationToken)
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
        
        var course = institution.Courses
            .FirstOrDefault(c => c.Steps.Select(s => s.Id).Contains(command.StepId.GetValueOrDefault()));
        if (course is null)
        {
            throw new NotFoundException("No course containing an step with the provided id was found in your institution");
        }
    
        var step = course.Steps.FirstOrDefault(s => s.Id == command.StepId);
        if (step is null)
        {
            throw new NotFoundException("No step with the provided id was found");
        }
        if (step.Activity is not null)
        {
            throw new BadRequestException("An activity already exists for the provided step");
        }
        
        var activity = new ActivityEntity
        {
            AllowedAttempts = command.AllowedAttempts.GetValueOrDefault()
        };
        
        step.Activity = activity;

        await context.SaveChangesAsync(cancellationToken);
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
    
    public async Task<ActivityViewModel> InsertQuestionAsync(InsertQuestionCommand command, HttpRequest request, CancellationToken cancellationToken)
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
        
        var course = institution.Courses
            .FirstOrDefault(c => c.Steps.Where(s => s.Activity is not null)
                .Select(s => s.Activity.Id).Contains(command.ActivityId.GetValueOrDefault()));
        if (course is null)
        {
            throw new NotFoundException("No course containing an activity with the provided id was found in your institution");
        }
        
        var step = course.Steps.Where(s => s.Activity is not null).FirstOrDefault(s => s.Activity.Id == command.ActivityId);
        if (step is null)
        {
            throw new NotFoundException("No activity with the provided id was found");
        }
        
        var activity = step.Activity;
        
        var question = new QuestionEntity
        {
            Statement = command.Statement!
        };

        foreach (var option in command.Options!)
        {
            var optionEntity = new OptionEntity
            {
                Text = option.Text,
                IsCorrect = option.IsCorrect
            };
            question.Options.Add(optionEntity);
        }

        activity.Questions.Add(question);

        await context.SaveChangesAsync(cancellationToken);
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

    public async Task<ActivityViewModel> UpdateActivityAsync(UpdateActivityCommand command, HttpRequest request, CancellationToken cancellationToken)
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

        var course = institution.Courses
            .FirstOrDefault(c => c.Steps.Where(s => s.Activity is not null)
                .Select(s => s.Activity.Id).Contains(command.ActivityId.GetValueOrDefault()));
        if (course is null)
        {
            throw new NotFoundException("No course containing an activity with the provided id was found in your institution");
        }
        
        var step = course.Steps.FirstOrDefault(s => s.Activity.Id == command.ActivityId);
        if (step is null)
        {
            throw new NotFoundException("No step containing an activity with the provided id was found in your institution");
        }
        
        var activity = step.Activity;

        if (command.AllowedAttempts is not null)
        {
            activity.AllowedAttempts = command.AllowedAttempts.GetValueOrDefault();
        }
        
        await context.SaveChangesAsync(cancellationToken);
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
    
    public async Task<AttemptResultViewModel> SubmitActivityAttemptAsync(SubmitActivityAttemptCommand command, HttpRequest request, CancellationToken cancellationToken)
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
            .Where(a => a.Id == command.ActivityId)
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
        
        var correctAnswers = 0;
        foreach (var answer in command.Answers)
        {
            var question = activity.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
            if (question is null)
            {
                throw new NotFoundException("No question with the provided id was found in the activity");
            }
            
            var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect == true);
            if (correctOption is null)
            {
                throw new NotFoundException("No correct option was found for the provided question");
            }
            
            if (correctOption.Id == answer.OptionId)
            {
                correctAnswers++;
            }
        }
        
        var attempt = new AttemptEntity
        {
            Score = correctAnswers,
            Date = DateTime.Now,
            UserId = user.Id,
            ActivityId = activity.Id
        };

        await context.Attempts.AddAsync(attempt, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    
        return new AttemptResultViewModel
        {
            Score = correctAnswers,
            Passed = correctAnswers >= activity.Questions.Count / 2
        };
    }
    
    public async Task<ActivityViewModel> UpdateQuestionAsync(UpdateQuestionCommand command, HttpRequest request, CancellationToken cancellationToken)
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

        var course = institution.Courses
            .FirstOrDefault(c => 
                c.Steps.Any(s => s.Activity.Questions.Select(q => q.Id)
                    .Contains(command.QuestionId.GetValueOrDefault()))
            );
        if (course is null)
        {
            throw new NotFoundException("No course containing an activity with the provided question was found in your institution");
        }
        
        var step = course.Steps.FirstOrDefault(s => s.Activity.Questions.Select(q => q.Id).Contains(command.QuestionId.GetValueOrDefault()));
        if (step is null)
        {
            throw new NotFoundException("No step containing an activity with the provided question was found in your institution");
        }
        
        var activity = step.Activity;
        
        var question = activity.Questions.FirstOrDefault(q => q.Id == command.QuestionId);
    
        if (command.Statement is not null)
        {
            question!.Statement = command.Statement;
        }
        
        if (command.Options is not null)
        {
            question!.Options.Clear();
            foreach (var option in command.Options!)
            {
                var optionEntity = new OptionEntity
                {
                    Text = option.Text,
                    IsCorrect = option.IsCorrect
                };
                question.Options.Add(optionEntity);
            }
        }
        
        await context.SaveChangesAsync(cancellationToken);
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

    
    public async Task DeleteActivityAsync(DeleteActivityCommand command, HttpRequest request, CancellationToken cancellationToken)
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

        var course = institution.Courses
            .FirstOrDefault(c => c.Steps.Where(s => s.Activity is not null)
                .Select(s => s.Activity.Id).Contains(command.ActivityId.GetValueOrDefault()));
        if (course is null)
        {
            throw new NotFoundException("No course containing an activity with the provided id was found in your institution");
        }
        
        var step = course.Steps.FirstOrDefault(s => s.Activity.Id == command.ActivityId);
        if (step is null)
        {
            throw new NotFoundException("No step containing an activity with the provided id was found in your institution");
        }
        
        var activity = step.Activity;
    
        context.Activities.Remove(activity);
        
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteQuestionAsync(DeleteQuestionCommand command, HttpRequest request, CancellationToken cancellationToken)
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

        var course = institution.Courses
            .FirstOrDefault(c => 
                c.Steps.Any(s => s.Activity.Questions.Select(q => q.Id)
                    .Contains(command.QuestionId.GetValueOrDefault()))
            );
        if (course is null)
        {
            throw new NotFoundException("No course containing an activity with the provided question was found in your institution");
        }
        
        var step = course.Steps.FirstOrDefault(s => s.Activity.Questions.Select(q => q.Id).Contains(command.QuestionId.GetValueOrDefault()));
        if (step is null)
        {
            throw new NotFoundException("No step containing an activity with the provided question was found in your institution");
        }
        
        var activity = step.Activity;
        
        var question = activity.Questions.FirstOrDefault(q => q.Id == command.QuestionId);
    
        context.Questions.Remove(question!);
        
        await context.SaveChangesAsync(cancellationToken);
    }

}