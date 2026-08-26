namespace SocialCareManager.Api.Dtos.CareTasks;

public class CareTaskFollowUpDto
{
    public Guid Id { get; set; }

    public Guid CareTaskId { get; set; }

    public string Note { get; set; } = string.Empty;

    public DateTime FollowedUpAt { get; set; }

    public string? FollowedUpBy { get; set; }

    public DateTime CreatedAt { get; set; }
}