using MySqlConnector;

namespace DocentifyAPI.Configurations;

public static class Dependencies
{
    public static IServiceCollection ConfigureDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .ConfigureHandlers()
            .ConfigureValidators()
            .ConfigureDatabase(configuration);
    }

    private static IServiceCollection ConfigureHandlers(this IServiceCollection services)
    {
        return services;
    }

    private static IServiceCollection ConfigureValidators(this IServiceCollection services)
    {
        return services;
    }

    private static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(_ => new MySqlConnection(configuration.GetConnectionString("Default")));
        return services;
    }
}