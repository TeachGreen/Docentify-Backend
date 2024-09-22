namespace DocentifyAPI.Configurations;

public static class Authentication
{
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
    {
         services
             .AddAuthorization()
             .AddAuthentication();

         return services;
    }
}