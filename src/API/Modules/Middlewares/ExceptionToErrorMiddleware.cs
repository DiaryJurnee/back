using System.Text.Json;
using Application.Common.Templates.Response;

namespace API.Modules.Middlewares;

public class ExceptionToErrorMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            Error error = Error.Create(StatusCodes.Status500InternalServerError, ErrorContent.Create("Internal server error: {0}", Error.ServerErrorsKey, ex.Message));

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