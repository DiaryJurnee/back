using FluentValidation;

namespace Application.Clusters.Commands;

public class UpdateClusterCommandValidator : AbstractValidator<UpdateClusterCommand>
{
    public UpdateClusterCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name)
            .MinimumLength(1).WithMessage("Name is required")
            .MaximumLength(255).WithMessage("Name is too long");
    }
}
