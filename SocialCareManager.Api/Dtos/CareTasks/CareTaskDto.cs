using SocialCareManager.Domain.Enums;

namespace SocialCareManager.Api.Dtos.CareTasks;

public class CareTaskDto
{
    public Guid Id { get; set; }

    public Guid ServiceUserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public CareTaskStatus Status { get; set; }

    public CareTaskPriority Priority { get; set; }

    public DateTime DueAt { get; set; }

    public string? AssignedTo { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public CareTaskRecurrence Recurrence { get; set; }

    public int RecurrenceInterval { get; set; }

    public DateTime? RecurrenceEndDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}