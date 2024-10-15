using AutoMapper;
using Docentify.Application.Courses.Commands;
using Docentify.Application.Courses.ViewModels;
using Docentify.Application.Utils;
using Docentify.Domain.Entities.Courses;
using Docentify.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Docentify.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Application.Courses.Handlers;

public class CourseCommandHandler(DatabaseContext context, IConfiguration configuration)
{
    public async Task<CourseViewModel> InsertCourseAsync(InsertCourseCommand command, HttpRequest request, CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        var institution = await context.Institutions
            .FirstOrDefaultAsync(i => i.Email == jwtData["email"], cancellationToken);

        if (institution is null)
        {
            throw new NotFoundException("No institution with the provided authentication was found");
        }

        var course = new CourseEntity();
        var mapper = new MapperConfiguration(cfg => 
            cfg.CreateMap<InsertCourseCommand, CourseEntity>()
                .ForAllMembers(opts => 
                    opts.Condition((src, dest, srcMember) => srcMember != null))
        ).CreateMapper();
        mapper.Map(command, course);
        
        institution.Courses.Add(course);
        
        await context.SaveChangesAsync(cancellationToken);
        return new CourseViewModel
        {
            Id = course.Id,
            Name = course.Name, 
            Description = course.Description,
            IsRequired = course.IsRequired,
            RequiredTimeLimit = course.RequiredTimeLimit
        };
    }
    
    public async Task<CourseViewModel> UpdateCourseAsync(UpdateCourseCommand command, HttpRequest request, CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        var institution = await context.Institutions
            .FirstOrDefaultAsync(i => i.Email == jwtData["email"], cancellationToken);

        if (institution is null)
        {
            throw new NotFoundException("No institution with the provided authentication was found");
        }
        
        var course = await context.Courses
            .FirstOrDefaultAsync(c => c.Id == command.GetId(), cancellationToken);
        if (course is null)
        {
            throw new NotFoundException("No course with the provided id was found");
        }
        
        var mapper = new MapperConfiguration(cfg => 
            cfg.CreateMap<UpdateCourseCommand, CourseEntity>()
                .ForAllMembers(opts => 
                    opts.Condition((src, dest, srcMember) => srcMember != null))
        ).CreateMapper();
        mapper.Map(command, course);
        
        course.UpdateDate = DateTime.Now;
        
        await context.SaveChangesAsync(cancellationToken);
        return new CourseViewModel
        {
            Id = command.GetId().GetValueOrDefault(),
            Name = course.Name, 
            Description = course.Description,
            IsRequired = course.IsRequired,
            RequiredTimeLimit = course.RequiredTimeLimit
        };
    }
    
    public async Task DeleteCourseAsync(CourseCommand command, HttpRequest request, CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        var institution = await context.Institutions
            .FirstOrDefaultAsync(i => i.Email == jwtData["email"], cancellationToken);

        if (institution is null)
        {
            throw new NotFoundException("No institution with the provided authentication was found");
        }
        
        var course = await context.Courses
            .FirstOrDefaultAsync(c => c.Id == command.CourseId, cancellationToken);
        if (course is null)
        {
            throw new NotFoundException("No course with the provided id was found");
        }

        context.Courses.Remove(course);
        
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task FavoriteCourseAsync(CourseCommand command, HttpRequest request, CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        var user = await context.Users
            .Include(u => u.Favorites)
            .FirstOrDefaultAsync(u => u.Email == jwtData["email"], cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("No user with the provided authentication was found");
        }

        var course = await context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == command.CourseId, cancellationToken);
        if (course is null)
        {
            throw new NotFoundException("No course with the provided id was found");
        }
        
        if (user.Favorites.FirstOrDefault(c => c.CourseId == course.Id) is null)
        {
            user.Favorites.Add(new FavoriteEntity
            {
                Course = course,
                User = user
            });
        }
        else
        {
            user.Favorites.Remove(user.Favorites.First(c => c.CourseId == course.Id));
        }
        
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task EnrollCourseAsync(CourseCommand command, HttpRequest request, CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        var user = await context.Users
            .Include(u => u.Enrollments)
            .FirstOrDefaultAsync(u => u.Email == jwtData["email"], cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("No user with the provided authentication was found");
        }

        var course = await context.Courses
            .FirstOrDefaultAsync(c => c.Id == command.CourseId, cancellationToken);
        if (course is null)
        {
            throw new NotFoundException("No course with the provided id was found");
        }
        
        var courseEnrollment = user.Enrollments.FirstOrDefault(c => c.CourseId == course.Id);
        if (courseEnrollment is null)
        {
            user.Enrollments.Add(new EnrollmentEntity
            {
                Course = course,
                User = user
            });
        }
        else if (!courseEnrollment.IsActive)
        {
            courseEnrollment.IsActive = true;
        }
        else
        {
            throw new ConflictException("User is already enrolled to course with the provided id");
        }
        
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DiscontinueCourseAsync(CourseCommand command, HttpRequest request, CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        var user = await context.Users
            .Include(u => u.Enrollments)
            .FirstOrDefaultAsync(u => u.Email == jwtData["email"], cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("No user with the provided authentication was found");
        }

        var course = await context.Courses
            .FirstOrDefaultAsync(c => c.Id == command.CourseId, cancellationToken);
        if (course is null)
        {
            throw new NotFoundException("No course with the provided id was found");
        }

        var courseEnrollment = user.Enrollments.FirstOrDefault(c => c.CourseId == course.Id);
        if (courseEnrollment is not null)
        {
            courseEnrollment.IsActive = false;
        }
        else
        {
            throw new NotFoundException("User is not enrolled to the course with the provided id");
        }
        
        await context.SaveChangesAsync(cancellationToken);
    }
}