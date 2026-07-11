using SocialCareManager.Domain.Common;

namespace SocialCareManager.Domain.Entities;

public class IncidentFollowUp : BaseEntity
{
    public Guid IncidentId { get; set; }

    public string Note { get; set; } = string.Empty;

    public DateTime FollowedUpAt { get; set; }

    public string? FollowedUpBy { get; set; }

    public Incident Incident { get; set; } = null!;
}