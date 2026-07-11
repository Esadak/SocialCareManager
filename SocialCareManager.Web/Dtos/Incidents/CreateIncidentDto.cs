namespace SocialCareManager.Web.Dtos;

public class CreateIncidentDto
{
    public string Title { get; set; } = string.Empty;

    public IncidentCategory Category { get; set; } = IncidentCategory.Other;
    public IncidentSeverity Severity { get; set; } = IncidentSeverity.Medium;

    public DateTime OccurredAt { get; set; } = DateTime.Now;

    public string Description { get; set; } = string.Empty;
    public string? ImmediateActions { get; set; }
    public string? PeopleInvolved { get; set; }

    public bool ManagerNotified { get; set; }
}