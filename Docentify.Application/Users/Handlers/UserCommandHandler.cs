using Docentify.Application.Users.Commands;
using Docentify.Application.Users.ValueObject;
using Docentify.Application.Users.ViewModels;
using Docentify.Application.Utils;
using Docentify.Domain.Entities;
using Docentify.Domain.Entities.User;
using Docentify.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Docentify.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Docentify.Application.Users.Handlers;

public class UserCommandHandler(DatabaseContext context, IConfiguration configuration)
{
    public async Task<UserViewModel> UpdateUserAsync(UpdateUserCommand command, HttpRequest request, CancellationToken cancellationToken)
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
        
        if (command.Name is not null)
            user.Name = command.Name;
        
        if (command.BirthDate != default)
            user.BirthDate = command.BirthDate;
        
        if (command.Email is not null)
            user.Email = command.Email;
        
        if (command.Telephone is not null)
            user.Telephone = command.Telephone;
        
        if (command.Document is not null)
            user.Document = command.Document;

        if (command.Gender is not null)
            user.Gender = command.Gender;

        context.Users.Update(user);
        
        await context.SaveChangesAsync(cancellationToken);
        return new UserViewModel
        {
            Name = user.Name, 
            BirthDate = user.BirthDate, 
            Email = user.Email, 
            Gender = user.Gender,
            Telephone = user.Telephone,
            Document = user.Document
        };
    }

    public async Task<UserPreferencesViewModel> UpdateUserPreferencesAsync(UpdateUserPreferencesCommand command, HttpRequest request, CancellationToken cancellationToken)
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
        
        var setUserPreferences = user.UserPreferencesValues
            .ToDictionary(p => p.Preference.Name, p => p.Value);
        var defaultPreferences = await context.UserPreferences.AsNoTracking()
            .ToDictionaryAsync(p => p.Name, p => p, cancellationToken);
        
        var updatedPreferences = new List<UserPreferencesValueEntity>();
        var newPreferences = new List<UserPreferencesValueEntity>();
        foreach (var preference in command.Preferences)
        {
            if (!defaultPreferences.ContainsKey(preference.Name))
                throw new NotFoundException($"No preference with the name '{preference.Name}' was found");
            
            if (setUserPreferences.ContainsKey(preference.Name))
            {
                updatedPreferences.Add(new UserPreferencesValueEntity
                {
                    Preference = defaultPreferences[preference.Name],
                    UserId = user.Id, 
                    PreferenceId = defaultPreferences[preference.Name].Id, 
                    Value = preference.Value
                });
            }
            else
            {
                newPreferences.Add(new UserPreferencesValueEntity
                {
                    Preference = defaultPreferences[preference.Name],
                    UserId = user.Id, 
                    PreferenceId = defaultPreferences[preference.Name].Id, 
                    Value = preference.Value
                });
            }
        }

        context.UserPreferencesValues.UpdateRange(updatedPreferences);
        await context.UserPreferencesValues.AddRangeAsync(newPreferences, cancellationToken);
        
        await context.SaveChangesAsync(cancellationToken);
        
        var result = updatedPreferences.Concat(newPreferences).Select(p => new PreferenceValueObject { Name = p.Preference.Name, Value = p.Value }).ToList();
            
        return new UserPreferencesViewModel
        {
            Preferences = result
        };
    }
}