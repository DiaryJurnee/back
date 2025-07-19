using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Templates.Response;

public class Success : IResponse
{
    private int _statusCode = StatusCodes.Status200OK;

    [JsonPropertyName("data")]
    public required object Data { get; init; }

    [JsonPropertyName("status")]
    public required int StatusCode
    {
        get => _statusCode;
        init
        {
            if (!IsSuccessCode(value))
                throw new ArgumentException($"Successful code must be >= {StatusCodes.Status200OK} and < {StatusCodes.Status300MultipleChoices}", nameof(Success));

            _statusCode = value;
        }
    }

    public static Success Create(int statusCode, object response) => new() { Data = response, StatusCode = statusCode };

    private static bool IsSuccessCode(int code) => code is >= StatusCodes.Status200OK and < StatusCodes.Status300MultipleChoices;
}
