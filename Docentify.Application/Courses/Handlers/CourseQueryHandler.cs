using Docentify.Application.Courses.Queries;
using Docentify.Application.Courses.ValueObjects;
using Docentify.Application.Courses.ViewModels;
using Docentify.Application.Utils;
using Docentify.Domain.Enums;
using Docentify.Domain.Exceptions;
using Docentify.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Docentify.Application.Courses.Handlers;

public class CourseQueryHandler(DatabaseContext context, IConfiguration configuration)
{
    public async Task<List<CourseViewModel>> QueryCoursesAsync(QueryCoursesQuery query, HttpRequest request,  CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        
        var user = await context.Users.AsNoTracking()
            .Include(u => u.Favorites)
            .Include(u => u.Enrollments)
            .ThenInclude(e => e.UserProgresses)
            .Include(u => u.Enrollments)
            .ThenInclude(e => e.Course)
            .ThenInclude(c => c.Steps)
            .Where(u => u.Email == jwtData["email"])
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("No user with the provided authentication was found");
        }

        var inProgressCourses = user.Enrollments
            .Where(enrollment => enrollment.UserProgresses.MaxBy(up => up.ProgressDate).StepId != enrollment.Course.Steps.MaxBy(s => s.Order).Id)
            .Select(enrollment => enrollment.CourseId);
        var completedCourses = user.Enrollments
            .Where(enrollment => enrollment.UserProgresses.MaxBy(up => up.ProgressDate).StepId != enrollment.Course.Steps.MaxBy(s => s.Order).Id)
            .Select(enrollment => enrollment.CourseId);
        var favoriteCourses = user.Favorites.Select(favorite => favorite.CourseId);

        var courses = context.Courses.AsNoTracking();

        switch (query.Progress)
        {
            case ECourseProgress.NotStarted:
                courses = courses.Where(c => !user.Enrollments.Select(e => e.CourseId).Contains(c.Id));
                break;
            case ECourseProgress.InProgress:
                courses = courses.Where(c => inProgressCourses.Contains(c.Id));
                break;
            case ECourseProgress.Completed:
                courses = courses.Where(c => completedCourses.Contains(c.Id));
                break;
        }

        if (!query.Name.IsNullOrEmpty())
        {
            courses = courses.Where(c => c.Name.Contains(query.Name!));
        }
        
        if (query.IsRequired.HasValue)
        {
            courses = courses.Where(c => c.IsRequired == query.IsRequired);
        }
        
        if (query.OnlyFavorites == true)
        {
            courses = courses.Where(c => favoriteCourses.Contains(c.Id));
        }
        
        return await courses
            .Skip((query.Page - 1) * query.Amount)
            .Take(query.Amount)
            .Select(c => new CourseViewModel()
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsRequired = c.IsRequired.GetValueOrDefault()
            })
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<CourseViewModel>> GetInstitutionCoursesAsync(GetInstitutionCoursesQuery query, HttpRequest request,  CancellationToken cancellationToken)
    {
        var courses = context.Courses.AsNoTracking()
            .Include(c => c.Institution)
            .Where(c => c.InstitutionId == query.InstitutionId);
        
        if (!query.Name.IsNullOrEmpty())
        {
            courses = courses.Where(c => c.Name.Contains(query.Name!));
        }
        
        if (query.IsRequired.HasValue)
        {
            courses = courses.Where(c => c.IsRequired == query.IsRequired);
        }
        
        return await courses
            .Skip((query.Page - 1) * query.Amount)
            .Take(query.Amount)
            .Select(c => new CourseViewModel()
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsRequired = c.IsRequired.GetValueOrDefault()
            })
            .ToListAsync(cancellationToken);
    }
    
    public async Task<CourseViewModel> GetCourseByIdAsync(GetCourseByIdQuery query, HttpRequest request,  CancellationToken cancellationToken)
    {
        var course = await context.Courses.AsNoTracking()
            .Where(c => c.Id == query.CourseId)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (course is null)
        {
            throw new NotFoundException("No course with the provided id was found");
        }

        return new CourseViewModel
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                IsRequired = course.IsRequired.GetValueOrDefault(),
                RequiredTimeLimit = course.RequiredTimeLimit
            };
    }
    
    public async Task<CourseViewModelWithSteps> GetCourseByIdWithStepsAsync(GetCourseByIdQuery query, HttpRequest request,  CancellationToken cancellationToken)
    {
        var course = await context.Courses.AsNoTracking()
            .Where(c => c.Id == query.CourseId)
            .Include(c => c.Steps)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (course is null)
        {
            throw new NotFoundException("No course with the provided id was found");
        }
        
        return new CourseViewModelWithSteps
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
            IsRequired = course.IsRequired.GetValueOrDefault(),
            Steps = course.Steps.Select(s => new StepValueObject
            {
                Id = s.Id,
                Description = s.Description,
                Order = s.Order,
                Type = s.Type
            }).ToList()
        };
    }
}