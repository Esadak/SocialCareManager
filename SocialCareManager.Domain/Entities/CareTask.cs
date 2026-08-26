using SocialCareManager.Domain.Common;
using SocialCareManager.Domain.Enums;

namespace SocialCareManager.Domain.Entities;

public class CareTask : BaseEntity
{
    public Guid ServiceUserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public CareTaskStatus Status { get; set; }
        = CareTaskStatus.ToDo;

    public CareTaskPriority Priority { get; set; }
        = CareTaskPriority.Normal;

    /// <summary>
    /// The date and time when the task should be completed.
    /// Stored as UTC.
    /// </summary>
    public DateTime DueAt { get; set; }

    /// <summary>
    /// Name or email of the staff member responsible for the task.
    /// </summary>
    public string? AssignedTo { get; set; }

    public DateTime? StartedAt { get; set; }

    public string? StartedBy { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? CompletedBy { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancelledBy { get; set; }

    public string? CancellationReason { get; set; }

    public CareTaskRecurrence Recurrence { get; set; }
        = CareTaskRecurrence.None;

    /// <summary>
    /// Example: every 1 week or every 2 weeks.
    /// </summary>
    public int RecurrenceInterval { get; set; } = 1;

    public DateTime? RecurrenceEndDate { get; set; }

    /// <summary>
    /// Links a generated recurring task to its original task.
    /// </summary>
    public Guid? ParentTaskId { get; set; }

    public ServiceUser ServiceUser { get; set; } = null!;

    public CareTask? ParentTask { get; set; }

    public ICollection<CareTask> GeneratedTasks { get; set; }
        = new List<CareTask>();

    public ICollection<CareTaskFollowUp> FollowUps { get; set; }
        = new List<CareTaskFollowUp>();
}