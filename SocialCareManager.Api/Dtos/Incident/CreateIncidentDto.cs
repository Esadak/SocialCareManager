using SocialCareManager.Domain.Enums;

namespace SocialCareManager.Api.Dtos.Incident;

public class CreateIncidentDto
{
    public string Title { get; set; } = string.Empty;

    public IncidentCategory Category { get; set; }

    public IncidentSeverity Severity { get; set; }

    public DateTime OccurredAt { get; set; }

    public string Description { get; set; } = string.Empty;

    public string? ImmediateActions { get; set; }

    public string? PeopleInvolved { get; set; }

    public bool ManagerNotified { get; set; }
}