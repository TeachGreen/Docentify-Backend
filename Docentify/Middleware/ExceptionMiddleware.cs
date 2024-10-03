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

            switch (error)
            {
                case ForbiddenException e:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    break;

                case UnauthorizedException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;

                case NotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                case ConflictException:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    break;
                
                case BadRequestException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var result = JsonSerializer.Serialize(new
            {
                statusCode = response.StatusCode,
                title = error.GetType().Name,
                error = error.Message,
            });

            await response.WriteAsync(result);
        }
    }
}