using System.Net;
using System.Text.Json;
using Docentify.Domain.Exceptions;

namespace DocentifyAPI.Middleware;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            response.StatusCode = error switch
            {
                ForbiddenException => (int)HttpStatusCode.Forbidden,
                UnauthorizedException => (int)HttpStatusCode.Unauthorized,
                NotFoundException => (int)HttpStatusCode.NotFound,
                ConflictException => (int)HttpStatusCode.Conflict,
                BadRequestException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var result = JsonSerializer.Serialize(new
            {
                statusCode = response.StatusCode,
                title = error.GetType().Name,
                error = error.Message,
                stackTrace = error.StackTrace
            });

            await response.WriteAsync(result);
        }
    }
}