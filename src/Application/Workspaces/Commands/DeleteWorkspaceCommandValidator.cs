using FluentValidation;

namespace Application.Workspaces.Commands;

public class DeleteWorkspaceCommandValidator : AbstractValidator<DeleteWorkspaceCommand>
{
    public DeleteWorkspaceCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
