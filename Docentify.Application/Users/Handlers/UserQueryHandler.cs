using Docentify.Application.Users.Queries;
using Docentify.Application.Users.ValueObject;
using Docentify.Application.Users.ViewModels;
using Docentify.Application.Utils;
using Docentify.Domain.Exceptions;
using Docentify.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Docentify.Application.Users.Handlers;

public class UserQueryHandler(DatabaseContext context, IConfiguration configuration)
{
    public async Task<UserViewModel> GetUserByIdAsync(GetUserQuery query, HttpRequest request,  CancellationToken cancellationToken)
    {
        var user = await context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == query.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("No user with that id was found");
        }

        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        if (jwtData["email"] == user.Email)
        {
            return new UserViewModel
            {
                Name = user.Name,
                BirthDate = user.BirthDate,
                Email = user.Email,
                Telephone = user.Telephone,
                Document = user.Document,
                Gender = user.Gender
            };            
        }
            
        return new UserViewModel
        {
            Name = user.Name,
            BirthDate = user.BirthDate,
            Email = user.Email,
            Gender = user.Gender
        };
    }
    
    public async Task<UserViewModel> GetUserAsync(HttpRequest request,  CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);

        var user = await context.Users.AsNoTracking()
            .Include(u => u.Enrollments)
            .ThenInclude(e => e.UserProgresses)
            .Include(u => u.Enrollments)
            .ThenInclude(e => e.Course)
            .ThenInclude(c => c.Steps)
            .FirstOrDefaultAsync(u => u.Email == jwtData["email"], cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("No user with the provided credentials was found");
        }
        
        var inProgressCourses = user.Enrollments
            .Where(enrollment => enrollment.UserProgresses.Count != enrollment.Course.Steps.Count)
            .Select(enrollment => enrollment.CourseId).Count();
        var completedCourses = user.Enrollments
            .Where(enrollment => enrollment.UserProgresses.Any() && enrollment.UserProgresses.MaxBy(up => up.ProgressDate).StepId != enrollment.Course.Steps.MaxBy(s => s.Order).Id)
            .Select(enrollment => enrollment.CourseId).Count();

        var cancelledEnrollments = user.Enrollments.Where(enrollment => !enrollment.IsActive).Count();
        
        return new UserViewModel
        {
            Name = user.Name,
            BirthDate = user.BirthDate,
            Email = user.Email,
            Telephone = user.Telephone,
            Document = user.Document,
            Gender = user.Gender,
            CompletedCourses = completedCourses,
            OngoingCourses = inProgressCourses,
            CancelledEnrollments = cancelledEnrollments
        };            
    }
    
    public async Task<UserPreferencesViewModel> GetUserPreferencesAsync(HttpRequest request,  CancellationToken cancellationToken)
    {
        var jwtData = JwtUtils.GetJwtDataFromRequest(request);
        var user = await context.Users.AsNoTracking()
            .Include(u => u.UserPreferencesValues)
            .ThenInclude(upv => upv.Preference)
            .FirstOrDefaultAsync(u => u.Email == jwtData["email"], cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("No user with the provided authentication was found");
        }
        
        var userPreferences = user.UserPreferencesValues
            .ToDictionary(p => p.Preference.Name, p => new PreferenceValueObject() { Name = p.Preference.Name, Value = p.Value });
        var defaultPreferences = await context.UserPreferences.AsNoTracking()
            .ToDictionaryAsync(p => p.Name, p => p.DefaultValue, cancellationToken);
        foreach (var preference in defaultPreferences
                     .Where(preference => !userPreferences.ContainsKey(preference.Key)))
        {
            userPreferences[preference.Key] = new PreferenceValueObject() { Name = preference.Key, Value = preference.Value };
        }
        
        return new UserPreferencesViewModel
        {
            Preferences = userPreferences.Values.ToList()
        };
    }
}