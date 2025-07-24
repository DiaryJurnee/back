using FluentValidation;

namespace Application.Clusters.Commands;

public class DeleteClusterCommandValidator : AbstractValidator<DeleteClusterCommand>
{
    public DeleteClusterCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
