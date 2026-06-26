namespace SocialCareManager.Web.Dtos;

public class CarePlanDto
{
    public Guid Id { get; set; }
    public Guid ServiceUserId { get; set; }

    public string Goal { get; set; } = string.Empty;
    public string Needs { get; set; } = string.Empty;
    public string SupportPlan { get; set; } = string.Empty;
    public string RiskAssessment { get; set; } = string.Empty;

    public DateTime ReviewDate { get; set; }
    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}