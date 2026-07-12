namespace SocialCareManager.Api.Dtos;

public class CarePlanHistoryDto
{
    public Guid Id { get; set; }
    public Guid ServiceUserId { get; set; }

    public string Goal { get; set; } = string.Empty;
    public string Needs { get; set; } = string.Empty;
    public string SupportPlan { get; set; } = string.Empty;
    public string RiskAssessment { get; set; } = string.Empty;

    public DateTime ReviewDate { get; set; }
    public bool IsActive { get; set; }

    public int VersionNumber { get; set; }
    public Guid? PreviousVersionId { get; set; }

    public DateTime? ArchivedAt { get; set; }
    public string? ArchivedBy { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}