using SocialCareManager.Domain.Common;

namespace SocialCareManager.Domain.Entities;

public class DailyNote : BaseEntity
{
    public Guid ServiceUserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;

    public string? UpdatedBy { get; set; }

    public ServiceUser? ServiceUser { get; set; }
}