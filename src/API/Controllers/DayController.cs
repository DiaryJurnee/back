using API.Dtos.Days;
using Application.Common.Interfaces.Queries;
using Application.Common.Templates.Response;
using Application.Days.Commands;
using CSharpFunctionalExtensions;
using Domain.Clusters;
using Domain.Days;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DayController(ISender sender, IBaseQuery<Day> dayQuery) : ControllerBase
    {
        [HttpGet]
        public async Task<Result<Success, Error>> Get([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var dayId = DayId.New(id);
            var result = await dayQuery.Get(cancellationToken, x => x.Id == dayId, x => x.Include(x => x.Cluster!));

            return result.Match<Result<Success, Error>>(
                day => Success.Create(StatusCodes.Status200OK, DayDto.FromDomainModel(day)),
                () => Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("Day not found", Error.ServerErrorsKey))
            );
        }

        [HttpGet("[action]")]
        public async Task<Result<Success, Error>> GetByCluster([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var clusterId = ClusterId.New(id);
            var result = await dayQuery.GetMany(cancellationToken, x => x.ClusterId == clusterId);

            return Success.Create(StatusCodes.Status200OK, result.Select(DayDto.FromDomainModel));
        }

        [HttpPost]
        public async Task<Result<Success, Error>> Create([FromForm] CreateDayDto dto, CancellationToken cancellationToken)
        {
            var input = new CreateDayCommand
            {
                Title = dto.Title,
                ClusterId = dto.ClusterId
            };

            var result = await sender.Send(input, cancellationToken);

            if (result.IsFailure)
                return result.Error;

            return Success.Create(StatusCodes.Status200OK, DayDto.FromDomainModel(result.Value));
        }

        [HttpPut]
        public async Task<Result<Success, Error>> Update([FromForm] UpdateDayDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateDayCommand
            {
                Id = dto.Id,
                Title = dto.Title
            };

            var result = await sender.Send(input, cancellationToken);

            if (result.IsFailure)
                return result.Error;

            return Success.Create(StatusCodes.Status200OK, DayDto.FromDomainModel(result.Value));
        }

        [HttpDelete]
        public async Task<Result<Success, Error>> Delete([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var input = new DeleteDayCommand
            {
                Id = id
            };

            var result = await sender.Send(input, cancellationToken);

            if (result.IsFailure)
                return result.Error;

            return Success.Create(StatusCodes.Status200OK, DayDto.FromDomainModel(result.Value));
        }
    }
}
