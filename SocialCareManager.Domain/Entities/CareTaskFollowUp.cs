using SocialCareManager.Domain.Common;

namespace SocialCareManager.Domain.Entities;

public class CareTaskFollowUp : BaseEntity
{
    public Guid CareTaskId { get; set; }

    public string Note { get; set; } = string.Empty;

    public DateTime FollowedUpAt { get; set; }

    public string? FollowedUpBy { get; set; }

    public CareTask CareTask { get; set; } = null!;
}