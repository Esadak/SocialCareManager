namespace SocialCareManager.Api.Dtos.Incident;

public class IncidentFollowUpDto
{
    public Guid Id { get; set; }

    public Guid IncidentId { get; set; }

    public string Note { get; set; } = string.Empty;

    public DateTime FollowedUpAt { get; set; }

    public string? FollowedUpBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }
}