using FluentValidation;

namespace Application.Uploads.Commands;

public class DeleteFileCommandValidator : AbstractValidator<DeleteFileCommand>
{
    public DeleteFileCommandValidator()
    {

    }
}
