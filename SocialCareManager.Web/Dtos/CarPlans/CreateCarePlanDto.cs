namespace SocialCareManager.Web.Dtos;

public class CreateCarePlanDto
{
    public string Goal { get; set; } = string.Empty;
    public string Needs { get; set; } = string.Empty;
    public string SupportPlan { get; set; } = string.Empty;
    public string RiskAssessment { get; set; } = string.Empty;

    public DateTime ReviewDate { get; set; } = DateTime.Today.AddMonths(6);
    public bool IsActive { get; set; } = true;
}