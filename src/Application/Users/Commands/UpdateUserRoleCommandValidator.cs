using FluentValidation;

namespace Application.Users.Commands;

public class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
{
    public UpdateUserRoleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.SystemRoleId).NotEmpty();
    }
}
