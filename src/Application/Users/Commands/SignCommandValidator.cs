using Application.Common.Interfaces.Services;
using Application.Common.Templates.Response;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Users.Commands;

public class SignCommandValidator : AbstractValidator<SignCommand>
{
    public SignCommandValidator(IGoogleTokenService googleToken)
    {
        RuleFor(x => x.TokenId).
            CustomAsync(async (tokenId, context, cancellationToken) =>
            {
                const string parameterName = nameof(tokenId);

                try
                {
                    await googleToken.ValidateAsync(tokenId, cancellationToken);
                }
                catch (Exception ex)
                {
                    context.AddFailure(new ValidationFailure(
                        propertyName: parameterName,
                        errorMessage: string.Empty)
                    {
                        CustomState = ErrorContent.Create("Failed to validate token: {0}", parameterName, ex.Message)
                    });
                }
            });
    }
}
