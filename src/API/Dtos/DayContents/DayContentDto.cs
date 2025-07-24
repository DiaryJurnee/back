using API.Dtos.Days;
using Domain.DayContents;

namespace API.Dtos.DayContents;

public record DayContentDto(Guid Id, string Text, DateTime? StartAt, DateTime? EndAt, DateTime CreatedAt, DateTime UpdatedAt, Guid DayId, DayDto? Day)
{
    public static DayContentDto FromDomainModel(DayContent dayContent) =>
        new(dayContent.Id.Value, dayContent.Text, dayContent.StartAt, dayContent.EndAt, dayContent.CreatedAt, dayContent.UpdatedAt, dayContent.DayId.Value,
            dayContent.Day is null ? null : DayDto.FromDomainModel(dayContent.Day));
}
