namespace SocialCareManager.Web.Dtos.DailyNotes;

public class DailyNoteDto
{
    public Guid Id { get; set; }
    public Guid ServiceUserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;

    public string? UpdatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}