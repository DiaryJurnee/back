using System.Text.Json;
using Application.Common.Templates.Response;

namespace API.Modules.Middlewares;

public class ExceptionToErrorMiddleware(RequestDelegate next)
{
    public const string UnhandledExceptionKey = "server";

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            Error error = Error.Create(StatusCodes.Status500InternalServerError, ErrorContent.Create("Internal server error: {0}", UnhandledExceptionKey, ex.Message));

            context.Response.StatusCode = error.StatusCode;
            context.Response.ContentType = "application/json";

            JsonSerializerOptions jsonSerializerOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            JsonSerializerOptions options = jsonSerializerOptions;

            var json = JsonSerializer.Serialize(error, options);

            await context.Response.WriteAsync(json);
        }
    }
}