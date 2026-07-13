namespace SocialCareManager.Web.Dtos.Dashboard;

public class DashboardActivityDto
{
    public Guid SourceId { get; set; }

    public Guid ServiceUserId { get; set; }

    public string ServiceUserName { get; set; } = string.Empty;

    public string ActivityType { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime OccurredAt { get; set; }

    public string? PerformedBy { get; set; }

    public string Icon { get; set; } = "bi bi-activity";

    public string TargetTab { get; set; } = "profile";
}