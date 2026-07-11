using SocialCareManager.Domain.Enums;

namespace SocialCareManager.Api.Dtos.Calendar;

public class CreateCalendarEventDto
{
    public string Title { get; set; } = string.Empty;

    public CalendarEventType EventType { get; set; }
        = CalendarEventType.Other;

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }

    public bool IsAllDay { get; set; }

    public string? Location { get; set; }

    public string? Description { get; set; }

    public string? AssignedTo { get; set; }

    public int? ReminderMinutesBefore { get; set; }
}