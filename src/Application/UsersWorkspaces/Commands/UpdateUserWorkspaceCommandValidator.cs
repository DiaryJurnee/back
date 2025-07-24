using FluentValidation;

namespace Application.UsersWorkspaces.Commands;

public class UpdateUserWorkspaceCommandValidator : AbstractValidator<UpdateUserWorkspaceCommand>
{
    public UpdateUserWorkspaceCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.WorkspaceId).NotEmpty();
    }
}
