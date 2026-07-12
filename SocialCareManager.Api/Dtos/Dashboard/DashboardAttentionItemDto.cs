namespace SocialCareManager.Api.Dtos.Dashboard;

public class DashboardAttentionItemDto
{
    public Guid ServiceUserId { get; set; }

    public string ServiceUserName { get; set; } = string.Empty;

    public Guid? SourceId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Severity { get; set; } = "warning";

    public DateTime? RelevantAt { get; set; }

    public string TargetTab { get; set; } = "profile";
}