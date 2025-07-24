using FluentValidation;

namespace Application.Users.Commands;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.FirstName)
            .MinimumLength(1).WithMessage("First name is required")
            .MaximumLength(255).WithMessage("First name is too long");

        RuleFor(x => x.LastName)
            .MinimumLength(1).WithMessage("Last name is required")
            .MaximumLength(255).WithMessage("Last name is too long");
    }
}
