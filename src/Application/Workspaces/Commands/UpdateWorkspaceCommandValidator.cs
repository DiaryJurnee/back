using FluentValidation;

namespace Application.Workspaces.Commands;

public class UpdateWorkspaceCommandValidator : AbstractValidator<UpdateWorkspaceCommand>
{
    public UpdateWorkspaceCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name)
            .MinimumLength(1).WithMessage("Name is required")
            .MaximumLength(255).WithMessage("Name is too long");
    }
}
