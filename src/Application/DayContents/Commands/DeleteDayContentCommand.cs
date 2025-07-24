using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Domain.DayContents;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.DayContents.Commands;

public class DeleteDayContentCommand : IRequest<Result<DayContent, Error>>
{
    public Guid Id { get; init; }
}

public class DeleteDayContentCommandHandler(IBaseQuery<DayContent> dayContentQuery, IBaseRepository<DayContent> dayContentRepository) : IRequestHandler<DeleteDayContentCommand, Result<DayContent, Error>>
{
    public async Task<Result<DayContent, Error>> Handle(DeleteDayContentCommand request, CancellationToken cancellationToken)
    {
        var id = DayContentId.New(request.Id);
        var dayContent = await dayContentQuery.Get(cancellationToken, x => x.Id == id);

        return await dayContent.Match<Task<Result<DayContent, Error>>>(
            async dayContentToDelete => await dayContentRepository.Delete(dayContentToDelete, cancellationToken),
            () => Task.FromResult(Result.Failure<DayContent, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("Day content not found", Error.ServerErrorsKey))))
        );
    }
}
