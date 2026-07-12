namespace SocialCareManager.Web.Dtos.Calendar;

public class CalendarOverviewItemDto
{
    public Guid SourceId { get; set; }

    public Guid ServiceUserId { get; set; }

    public CalendarItemSource Source { get; set; }

    public string Title { get; set; } = string.Empty;

    public string TypeText { get; set; } = string.Empty;

    public string StatusText { get; set; } = string.Empty;

    public DateTime StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    public bool IsAllDay { get; set; }

    public string? Location { get; set; }

    public string? Description { get; set; }

    public string? ResponsiblePerson { get; set; }

    public bool RequiresAttention { get; set; }
}