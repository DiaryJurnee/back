using Application.Common.Interfaces.Repositories;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Domain.DayContents;
using Domain.Days;
using Mediator.Interfaces;

namespace Application.DayContents.Commands;

public class CreateDayContentCommand : IRequest<Result<DayContent, Error>>
{
    public string Text { get; init; } = string.Empty;
    public Guid DayId { get; init; }
    public DateTime? StartAt { get; init; } = null;
    public DateTime? EndAt { get; init; } = null;
}

public class CreateDayContentCommandHandler(IBaseRepository<DayContent> dayContentRepository) : IRequestHandler<CreateDayContentCommand, Result<DayContent, Error>>
{
    public async Task<Result<DayContent, Error>> Handle(CreateDayContentCommand request, CancellationToken cancellationToken)
    {
        var dayContent = DayContent.New(DayContentId.New(Guid.NewGuid()), request.Text, request.StartAt, request.EndAt, DayId.New(request.DayId));

        var result = await dayContentRepository.Create(dayContent, cancellationToken);

        return result;
    }
}
