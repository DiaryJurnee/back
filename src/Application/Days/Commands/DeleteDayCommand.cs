using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Domain.Days;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Days.Commands;

public class DeleteDayCommand : IRequest<Result<Day, Error>>
{
    public Guid Id { get; init; }
}

public class DeleteDayCommandHandler(IBaseQuery<Day> dayQuery, IBaseRepository<Day> dayRepository) : IRequestHandler<DeleteDayCommand, Result<Day, Error>>
{
    public async Task<Result<Day, Error>> Handle(DeleteDayCommand request, CancellationToken cancellationToken)
    {
        var id = DayId.New(request.Id);

        var day = await dayQuery.Get(cancellationToken, x => x.Id == id);

        return await day.Match<Task<Result<Day, Error>>>(
            async dayToDelete => await dayRepository.Delete(dayToDelete, cancellationToken),
            () => Task.FromResult(Result.Failure<Day, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("Day not found", Error.ServerErrorsKey))))
        );
    }
}
