using Docentify.Application.Users.Handlers;
using Docentify.Application.Authentication.Handlers;
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
        services.AddScoped<AuthenticationCommandHandler>();
        
        services.AddScoped<AuthenticationQueryHandler>();
        
        return services;
    }

    private static IServiceCollection ConfigureValidators(this IServiceCollection services)
    {
        return services;
    }
}