using AutoMapper;
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
        var user = await context.Users
            .Include(u => u.UserPreferencesValues)
            .ThenInclude(upv => upv.Preference)
            .FirstOrDefaultAsync(u => u.Email == jwtData["email"], cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("No user with the provided authentication was found");
        }

        var mapper = new MapperConfiguration(cfg => 
                cfg.CreateMap<UpdateUserCommand, UserEntity>()
                    .ForAllMembers(opts => 
                        opts.Condition((src, dest, srcMember) => srcMember != null))
            ).CreateMapper();
        mapper.Map(command, user);
        
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
        var user = await context.Users
            .Include(u => u.UserPreferencesValues)
            .ThenInclude(upv => upv.Preference)
            .FirstOrDefaultAsync(u => u.Email == jwtData["email"], cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("No user with the provided authentication was found");
        }
        
        var changedPreferences = command.Preferences
            .ToDictionary(p => p.Name, p => p);
        foreach (var userPreference in user.UserPreferencesValues
            .Where(upv => changedPreferences.Keys.Contains(upv.Preference.Name)))
        {
            userPreference.Value = changedPreferences[userPreference.Preference.Name].Value;
        }
        
        var defaultPreferences = await context.UserPreferences.AsNoTracking()
            .ToDictionaryAsync(p => p.Name, p => p, cancellationToken);
        foreach (var userPreference in changedPreferences.Keys
            .Where(name => user.UserPreferencesValues.All(upv => upv.Preference.Name != name)))
        {
            user.UserPreferencesValues.Add(new UserPreferencesValueEntity
                {
                    Preference = defaultPreferences[userPreference],
                    User = user,
                    Value = changedPreferences[userPreference].Value
                }
            );
        }
        
        await context.SaveChangesAsync(cancellationToken);
        
        var result = user.UserPreferencesValues.Select(p => new PreferenceValueObject { Name = p.Preference.Name, Value = p.Value }).ToList();
        return new UserPreferencesViewModel
        {
            Preferences = result
        };
    }
}