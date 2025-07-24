using FluentValidation;

namespace Application.DayContents.Commands;

public class UpdateDayContentCommandValidator : AbstractValidator<UpdateDayContentCommand>
{
    public UpdateDayContentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Text)
            .MinimumLength(1).WithMessage("Text is required")
            .MaximumLength(3000).WithMessage("Text is too long");

        RuleFor(x => x.StartAt)
            .LessThan(x => x.EndAt)
            .When(x => x.EndAt.HasValue && x.StartAt.HasValue)
            .WithMessage("Start date must be before end date");
    }
}
