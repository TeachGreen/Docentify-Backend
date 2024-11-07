using AutoMapper;
using Docentify.Application.Steps.Commands;
using Docentify.Application.Steps.ViewModels;
using Docentify.Application.Utils;
using Docentify.Domain.Entities.Step;
using Docentify.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Docentify.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Application.Steps.Handlers;

public class StepCommandHandler(DatabaseContext context, IConfiguration configuration)
{
    public async Task<StepViewModel> InsertStepAsync(InsertStepCommand command, HttpRequest request, CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        var institution = await context.Institutions
            .Include(i => i.Courses)
            .ThenInclude(c => c.Steps)
            .FirstOrDefaultAsync(i => i.Email == jwtData["email"], cancellationToken);

        if (institution is null)
        {
            throw new NotFoundException("No institution with the provided authentication was found");
        }
        
        var course = institution.Courses
            .FirstOrDefault(c => c.Id == command.CourseId);
        if (course is null)
        {
            throw new NotFoundException("No course with the provided id was found");
        }

        var step = new StepEntity();
        var mapper = new MapperConfiguration(cfg => 
            cfg.CreateMap<InsertStepCommand, StepEntity>()
                .ForAllMembers(opts => 
                    opts.Condition((src, dest, srcMember) => srcMember != null))
        ).CreateMapper();
        mapper.Map(command, step);
        
        var lastStep = course.Steps.OrderByDescending(s => s.Order).FirstOrDefault();
        step.Order = lastStep?.Order + 1 ?? 1;
        
        course.Steps.Add(step);
        
        await context.SaveChangesAsync(cancellationToken);
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
    
    public async Task<StepViewModel> UpdateStepAsync(UpdateStepCommand command, HttpRequest request, CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        var institution = await context.Institutions
            .Include(i => i.Courses)
            .ThenInclude(c => c.Steps)
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
        
        var mapper = new MapperConfiguration(cfg => 
            cfg.CreateMap<UpdateStepCommand, StepEntity>()
                .ForAllMembers(opts => 
                    opts.Condition((src, dest, srcMember) => srcMember != null))
        ).CreateMapper();
        mapper.Map(command, step);
        
        await context.SaveChangesAsync(cancellationToken);
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
    
    public async Task DeleteStepAsync(DeleteStepCommand command, HttpRequest request, CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        var institution = await context.Institutions
            .Include(i => i.Courses)
            .ThenInclude(c => c.Steps)
            .FirstOrDefaultAsync(i => i.Email == jwtData["email"], cancellationToken);
        if (institution is null)
        {
            throw new NotFoundException("No institution with the provided authentication was found");
        }
        
        var course = institution.Courses
            .FirstOrDefault(c => c.Steps.Select(s => s.Id).Contains(command.StepId));
        if (course is null)
        {
            throw new NotFoundException("No course containing an step with the provided id was found in your institution");
        }
        
        var step = course.Steps.FirstOrDefault(s => s.Id == command.StepId);
        if (step is null)
        {
            throw new NotFoundException("No step with the provided id was found");
        }

        context.Steps.Remove(step);
        
        await context.SaveChangesAsync(cancellationToken);
    }
}