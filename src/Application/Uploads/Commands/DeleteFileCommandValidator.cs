using Application.Common.Templates.Response;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Uploads.Commands;

public class DeleteFileCommandValidator : AbstractValidator<DeleteFileCommand>
{
    public DeleteFileCommandValidator()
    {
        RuleFor(x => x.Path)
            .Custom((fileName, context) =>
            {
                const string parameterName = nameof(fileName);

                if (!File.Exists(fileName))
                {
                    context.AddFailure(new ValidationFailure(
                        propertyName: parameterName,
                        errorMessage: string.Empty)
                    {
                        CustomState = ErrorContent.Create("File not found", parameterName)
                    });
                }
            });
    }
}
