using Docentify.Application.Activities.Handlers;
using Docentify.Application.Authentication.Handlers;
using Docentify.Application.Authentication.Validators;
using Docentify.Application.Courses.Handlers;
using Docentify.Application.Institutions.Handlers;
using Docentify.Application.Steps.Handlers;
using Docentify.Application.Users.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Docentify.Application;

public static class ApplicationModule
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services)
    {
        return services
            .ConfigureHandlers()
            .ConfigureValidators();
    }

    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        services.AddScoped<ActivityCommandHandler>();
        services.AddScoped<AuthenticationCommandHandler>();
        services.AddScoped<CourseCommandHandler>();
        services.AddScoped<InstitutionCommandHandler>();
        services.AddScoped<StepCommandHandler>();
        services.AddScoped<UserCommandHandler>();
        
        services.AddScoped<ActivityQueryHandler>();
        services.AddScoped<AuthenticationQueryHandler>();
        services.AddScoped<CourseQueryHandler>();
        services.AddScoped<InstitutionQueryHandler>();
        services.AddScoped<StepQueryHandler>();
        services.AddScoped<UserQueryHandler>();
        
        return services;
    }

    private static IServiceCollection ConfigureValidators(this IServiceCollection services)
    {
        return services
            .AddScoped<LoginCommandValidator>()
            .AddScoped<RegisterInstitutionCommandValidator>()
            .AddScoped<RegisterUserCommandValidator>();
    }
}