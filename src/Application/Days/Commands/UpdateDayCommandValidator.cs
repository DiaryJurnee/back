using FluentValidation;

namespace Application.Days.Commands;

public class UpdateDayCommandValidator : AbstractValidator<UpdateDayCommand>
{
    public UpdateDayCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Title)
            .MinimumLength(1).WithMessage("Title is required")
            .MaximumLength(255).WithMessage("Title is too long");
    }
}
