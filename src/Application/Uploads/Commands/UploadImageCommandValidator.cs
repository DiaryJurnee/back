using Application.Common.Templates.Response;
using FluentValidation;
using FluentValidation.Results;

namespace Application.Uploads.Commands;

public class UploadImageCommandValidator : AbstractValidator<UploadImageCommand>
{
    private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".gif"];
    private const long _megabyte = 1024 * 1024;
    private const long _maxFileSize = 3 * _megabyte;

    public UploadImageCommandValidator()
    {
        RuleFor(x => x.File)
            .Must(file => file.Length <= _maxFileSize)
            .WithMessage("File size limit exceeded");

        RuleFor(x => x.Extension)
            .Custom((extension, context) =>
            {
                const string parameterName = nameof(extension);

                if (!_allowedExtensions.Contains(extension))
                {
                    context.AddFailure(new ValidationFailure(
                        propertyName: parameterName,
                        errorMessage: string.Empty)
                    {
                        CustomState = ErrorContent.Create("Invalid file extension: {0}. Only {1} are allowed", parameterName, extension, string.Join(", ", _allowedExtensions))
                    });
                }
            });
    }
}
