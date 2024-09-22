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
        services.AddScoped<UserCommandHandler>();
        
        services.AddScoped<UserQueryHandler>();
        
        return services;
    }

    private static IServiceCollection ConfigureValidators(this IServiceCollection services)
    {
        return services;
    }
}