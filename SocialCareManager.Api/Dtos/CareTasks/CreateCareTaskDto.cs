using SocialCareManager.Domain.Enums;

public class CreateCareTaskDto
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime DueAt { get; set; }

    public CareTaskPriority Priority { get; set; }

    public string? AssignedTo { get; set; }

    public CareTaskRecurrence Recurrence { get; set; }

    public int RecurrenceInterval { get; set; } = 1;

    public DateTime? RecurrenceEndDate { get; set; }
}