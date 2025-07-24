using FluentValidation;

namespace Application.Days.Commands;

public class DeleteDayCommandValidator : AbstractValidator<DeleteDayCommand>
{
    public DeleteDayCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
