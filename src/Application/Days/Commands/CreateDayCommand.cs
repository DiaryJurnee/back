using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Domain.Clusters;
using Domain.Days;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Days.Commands;

public class CreateDayCommand : IRequest<Result<Day, Error>>
{
    public string Title { get; init; } = string.Empty;
    public Guid ClusterId { get; init; }
}

public class CreateDayCommandHandler(IBaseRepository<Day> dayRepository, IBaseQuery<Cluster> clusterQuery) : IRequestHandler<CreateDayCommand, Result<Day, Error>>
{
    public async Task<Result<Day, Error>> Handle(CreateDayCommand request, CancellationToken cancellationToken)
    {
        var clusterId = ClusterId.New(request.ClusterId);
        var cluster = await clusterQuery.Get(cancellationToken, x => x.Id == clusterId);

        return await cluster.Match<Task<Result<Day, Error>>>(
            async cluster =>
            {
                var day = Day.New(DayId.New(Guid.NewGuid()), request.Title, cluster.Id);

                var result = await dayRepository.Create(day, cancellationToken);

                return result;
            },
            () => Task.FromResult(Result.Failure<Day, Error>(Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("Cluster not found", Error.ServerErrorsKey))))
        );
    }
}
