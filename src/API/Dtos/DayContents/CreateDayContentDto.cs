namespace API.Dtos.DayContents;

public record CreateDayContentDto(string Text, DateTime? StartAt, DateTime? EndAt);
