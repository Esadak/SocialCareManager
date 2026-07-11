using SocialCareManager.Domain.Common;
using SocialCareManager.Domain.Enums;

namespace SocialCareManager.Domain.Entities;

public class CalendarEvent : BaseEntity
{
    public Guid ServiceUserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public CalendarEventType EventType { get; set; }
        = CalendarEventType.Other;

    public CalendarEventStatus Status { get; set; }
        = CalendarEventStatus.Scheduled;

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }

    public bool IsAllDay { get; set; }

    public string? Location { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// Name or email of the staff member responsible for the activity.
    /// </summary>
    public string? AssignedTo { get; set; }

    /// <summary>
    /// Number of minutes before the event when a reminder should appear.
    /// Null means no reminder.
    /// </summary>
    public int? ReminderMinutesBefore { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? CompletedBy { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancelledBy { get; set; }

    public string? CancellationReason { get; set; }

    public ServiceUser ServiceUser { get; set; } = null!;
}