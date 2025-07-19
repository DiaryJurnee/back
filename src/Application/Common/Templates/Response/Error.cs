using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Templates.Response;

public class Error : IResponse
{
    private int _statusCode = StatusCodes.Status500InternalServerError;

    [JsonPropertyName("errors")]
    public required Dictionary<string, object> Errors { get; init; } = null!;

    [JsonPropertyName("status")]
    public required int StatusCode
    {
        get => _statusCode;
        init
        {
            if (!IsErrorCode(value))
                throw new ArgumentException($"Error code must be >= {StatusCodes.Status400BadRequest} and <= {StatusCodes.Status511NetworkAuthenticationRequired}", nameof(Error));

            _statusCode = value;
        }
    }

    public static Error Create(int statusCode, params ErrorContent[] errors) => new()
    {
        Errors = errors.ToDictionary(x => x.Key, x => CreateError(x.Message, x.Args)),
        StatusCode = statusCode
    };

    private static object CreateError(string message, string[] args) => new { message, args };

    public static bool IsErrorCode(int code) => code is >= StatusCodes.Status400BadRequest and <= StatusCodes.Status511NetworkAuthenticationRequired;
}

public readonly record struct ErrorContent(string Message = null!, string Key = null!, string[] Args = default!)
{
    public static ErrorContent Create(string message, string key, params string[] args) => new()
    {
        Message = message,
        Key = key,
        Args = args
    };
}