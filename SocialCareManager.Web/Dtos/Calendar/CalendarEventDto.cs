namespace SocialCareManager.Web.Dtos.Calendar;

public class CalendarEventDto
{
    public Guid Id { get; set; }
    public Guid ServiceUserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public CalendarEventType EventType { get; set; }
    public CalendarEventStatus Status { get; set; }

    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }

    public bool IsAllDay { get; set; }

    public string? Location { get; set; }
    public string? Description { get; set; }
    public string? AssignedTo { get; set; }

    public int? ReminderMinutesBefore { get; set; }

    public DateTime? CompletedAt { get; set; }
    public string? CompletedBy { get; set; }

    public DateTime? CancelledAt { get; set; }
    public string? CancelledBy { get; set; }
    public string? CancellationReason { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}