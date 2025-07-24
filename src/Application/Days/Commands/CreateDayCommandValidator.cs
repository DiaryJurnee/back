using FluentValidation;

namespace Application.Days.Commands;

public class CreateDayCommandValidator : AbstractValidator<CreateDayCommand>
{
    public CreateDayCommandValidator()
    {
        RuleFor(x => x.Title)
            .MinimumLength(1).WithMessage("Title is required")
            .MaximumLength(255).WithMessage("Title is too long");
        RuleFor(x => x.ClusterId).NotEmpty();
    }
}
