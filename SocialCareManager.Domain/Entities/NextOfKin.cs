using SocialCareManager.Domain.Common;

namespace SocialCareManager.Domain.Entities;

public class NextOfKin : BaseEntity
{
    public Guid ServiceUserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Relationship { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public bool IsEmergencyContact { get; set; }

    public ServiceUser? ServiceUser { get; set; }
}