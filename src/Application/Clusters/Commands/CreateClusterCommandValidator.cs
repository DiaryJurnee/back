using FluentValidation;

namespace Application.Clusters.Commands;

public class CreateClusterCommandValidator : AbstractValidator<CreateClusterCommand>
{
    public CreateClusterCommandValidator()
    {
        RuleFor(x => x.Name).
            MinimumLength(1).WithMessage("Name is required")
            .MaximumLength(255).WithMessage("Name is too long");

        RuleFor(x => x.WorkspaceId).NotEmpty();

        RuleFor(x => x.OwnerId).NotEmpty();
    }
}
