using FluentValidation;

namespace Application.Workspaces.Commands;

public class CreateWorkspaceCommandValidator : AbstractValidator<CreateWorkspaceCommand>
{
    public CreateWorkspaceCommandValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(1).WithMessage("Name is required")
            .MaximumLength(255).WithMessage("Name is too long");

        RuleFor(x => x.OwnerId).NotEmpty();
    }
}
