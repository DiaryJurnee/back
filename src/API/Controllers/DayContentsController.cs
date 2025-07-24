using API.Dtos.DayContents;
using Application.Common.Interfaces.Queries;
using Application.Common.Templates.Response;
using Application.DayContents.Commands;
using CSharpFunctionalExtensions;
using Domain.DayContents;
using Domain.Days;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DayContentsController(ISender sender, IBaseQuery<DayContent> dayContentQuery) : ControllerBase
    {
        [HttpGet]
        public async Task<Result<Success, Error>> Get([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var dayContentId = DayContentId.New(id);
            var result = await dayContentQuery.Get(cancellationToken, x => x.Id == dayContentId, x => x.Include(x => x.Day)!);

            return result.Match<Result<Success, Error>>(
                dayContent => Success.Create(StatusCodes.Status200OK, DayContentDto.FromDomainModel(dayContent)),
                () => Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("Day content not found", Error.ServerErrorsKey))
            );
        }

        [HttpGet("[action]")]
        public async Task<Result<Success, Error>> GetByDay([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var dayId = DayId.New(id);
            var result = await dayContentQuery.GetMany(cancellationToken, x => x.DayId == dayId);

            return Success.Create(StatusCodes.Status200OK, result.Select(DayContentDto.FromDomainModel));
        }


        [HttpPost]
        public async Task<Result<Success, Error>> Create([FromForm] CreateDayContentDto dto, CancellationToken cancellationToken)
        {
            var input = new CreateDayContentCommand
            {
                Text = dto.Text,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
            };

            var result = await sender.Send(input, cancellationToken);

            if (result.IsFailure)
                return result.Error;

            return Success.Create(StatusCodes.Status200OK, DayContentDto.FromDomainModel(result.Value));
        }

        [HttpPut]
        public async Task<Result<Success, Error>> Update([FromForm] UpdateDayContentDto dto, CancellationToken cancellationToken)
        {
            var input = new UpdateDayContentCommand
            {
                Id = dto.Id,
                Text = dto.Text,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
            };

            var result = await sender.Send(input, cancellationToken);

            if (result.IsFailure)
                return result.Error;

            return Success.Create(StatusCodes.Status200OK, DayContentDto.FromDomainModel(result.Value));
        }

        [HttpDelete]
        public async Task<Result<Success, Error>> Delete([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var input = new DeleteDayContentCommand
            {
                Id = id
            };

            var result = await sender.Send(input, cancellationToken);

            if (result.IsFailure)
                return result.Error;

            return Success.Create(StatusCodes.Status200OK, DayContentDto.FromDomainModel(result.Value));
        }
    }
}
