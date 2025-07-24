using FluentValidation;

namespace Application.UsersWorkspaces.Commands;

public class DeleteUserWorkspaceCommandValidator : AbstractValidator<DeleteUserWorkspaceCommand>
{
    public DeleteUserWorkspaceCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.WorkspaceId).NotEmpty();
    }
}
