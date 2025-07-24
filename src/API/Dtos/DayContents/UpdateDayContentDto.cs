namespace API.Dtos.DayContents;

public record UpdateDayContentDto(Guid Id, string Text, DateTime? StartAt, DateTime? EndAt);
