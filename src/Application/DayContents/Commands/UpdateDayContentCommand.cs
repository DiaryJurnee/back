using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Domain.DayContents;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.DayContents.Commands;

public class UpdateDayContentCommand : IRequest<Result<DayContent, Error>>
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime? StartAt { get; set; } = null;
    public DateTime? EndAt { get; set; } = null;
}

public class UpdateDayContentCommandHandler(IBaseQuery<DayContent> dayContentQuery, IBaseRepository<DayContent> dayContentRepository) : IRequestHandler<UpdateDayContentCommand, Result<DayContent, Error>>
{
    public async Task<Result<DayContent, Error>> Handle(UpdateDayContentCommand request, CancellationToken cancellationToken)
    {
        var id = DayContentId.New(request.Id);
        var dayContent = await dayContentQuery.Get(cancellationToken, x => x.Id == id);

        return await dayContent.Match<Task<Result<DayContent, Error>>>(
            async dayContentToUpdate =>
            {
                dayContentToUpdate.UpdateDetails(request.Text, request.StartAt, request.EndAt);
                return await dayContentRepository.Update(dayContentToUpdate, cancellationToken);
            },
            () => Task.FromResult(Result.Failure<DayContent, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("Day content not found", Error.ServerErrorsKey))))
        );
    }
}
