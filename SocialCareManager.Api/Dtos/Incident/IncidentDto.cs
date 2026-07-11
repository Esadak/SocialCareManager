using SocialCareManager.Domain.Enums;

namespace SocialCareManager.Api.Dtos.Incident;

public class IncidentDto
{
    public Guid Id { get; set; }
    public Guid ServiceUserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public IncidentCategory Category { get; set; }

    public IncidentSeverity Severity { get; set; }

    public IncidentStatus Status { get; set; }

    public DateTime OccurredAt { get; set; }

    public string Description { get; set; } = string.Empty;

    public string? ImmediateActions { get; set; }

    public string? PeopleInvolved { get; set; }

    public bool ManagerNotified { get; set; }

    public DateTime? ManagerNotifiedAt { get; set; }

    public string? ManagerNotifiedBy { get; set; }

    public string? Outcome { get; set; }

    public DateTime? ClosedAt { get; set; }

    public string? ClosedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public List<IncidentFollowUpDto> FollowUps { get; set; } = new();
}