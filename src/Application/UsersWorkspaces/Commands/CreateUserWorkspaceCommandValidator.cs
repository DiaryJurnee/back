using FluentValidation;

namespace Application.UsersWorkspaces.Commands;

public class CreateUserWorkspaceCommandValidator : AbstractValidator<CreateUserWorkspaceCommand>
{
    public CreateUserWorkspaceCommandValidator()
    {
        RuleFor(x => x.WorkspaceId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
