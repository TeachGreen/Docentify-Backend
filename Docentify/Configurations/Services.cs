namespace DocentifyAPI.Configurations;

public static class Services
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

        return services;
    }
}