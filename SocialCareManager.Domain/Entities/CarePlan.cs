using SocialCareManager.Domain.Common;

namespace SocialCareManager.Domain.Entities;

public class CarePlan : BaseEntity
{
    public Guid ServiceUserId { get; set; }

    public ServiceUser ServiceUser { get; set; } = null!;

    // What does the service user want to achieve?
   // What does the service user want to achieve?
    public string Goal { get; set; } = string.Empty;

    // What support needs does the service user have?
    public string Needs { get; set; } = string.Empty;

    // How should staff provide support?
    public string SupportPlan { get; set; } = string.Empty;

    // Risks to be aware of
    public string RiskAssessment { get; set; } = string.Empty;

    // When should this plan be reviewed?
    public DateTime ReviewDate { get; set; }

    public int VersionNumber { get; set; } = 1;

   public Guid? PreviousVersionId { get; set; }

   public DateTime? ArchivedAt { get; set; }

  public string? ArchivedBy { get; set; }

    // Only one active plan should normally exist
    public bool IsActive { get; set; } = true;
}