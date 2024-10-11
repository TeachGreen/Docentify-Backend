namespace Docentify.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .ConfigureDatabase(configuration)
            .ConfigureRepositories();
    }

    private static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseMySQL(configuration["ConnectionStrings:Default"]!);
        });
        
        return services;
    }
    
    private static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        return services;
    }
}