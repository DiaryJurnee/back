using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using FluentValidation;
using Mediator.Delegates;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Behaviours;

public class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse> where TResponse : IResult<Success, Error>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.Where(v => v.Errors.Count != 0).SelectMany(v => v.Errors).ToList();

            if (failures.Count != 0)
            {
                var errors = failures
                    .Select(f =>
                    {
                        var content = f.CustomState;

                        if (content is null)
                            return ErrorContent.Create(f.ErrorMessage, ToCamelCase(f.PropertyName));

                        return (ErrorContent)content;
                    })
                    .ToArray();

                return (TResponse)(object)Result.Failure<Success, Error>(Error.Create(StatusCodes.Status400BadRequest, errors));
            }
        }

        return await next();
    }

    // TODO: Refactor
    private static string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
            return input;

        return char.ToLowerInvariant(input[0]) + input[1..];
    }
}