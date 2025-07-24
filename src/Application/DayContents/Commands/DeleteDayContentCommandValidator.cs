using FluentValidation;

namespace Application.DayContents.Commands;

public class DeleteDayContentCommandValidator : AbstractValidator<DeleteDayContentCommand>
{
    public DeleteDayContentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
