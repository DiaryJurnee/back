using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Modules.Filters;

public class ApiResultFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is ObjectResult objectResult && objectResult.Value is not null)
        {
            var value = objectResult.Value;
            var type = value.GetType();

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<,>))
            {
                var isSuccessProp = type.GetProperty("IsSuccess");
                var valueProp = type.GetProperty("Value");
                var errorProp = type.GetProperty("Error");

                if (isSuccessProp?.GetValue(value) is bool isSuccess)
                {
                    var resultData = isSuccess ? valueProp?.GetValue(value) : errorProp?.GetValue(value);

                    if (resultData is IResponse response)
                        context.Result = new ObjectResult(response)
                        {
                            StatusCode = response.StatusCode
                        };
                    else
                        context.Result = new ObjectResult(resultData)
                        {
                            StatusCode = isSuccess ? StatusCodes.Status200OK : StatusCodes.Status510NotExtended
                        };
                }
            }
            else if (value is IResponse response)
            {
                context.Result = new ObjectResult(response)
                {
                    StatusCode = response.StatusCode
                };
            }
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        // no-op
    }
}
