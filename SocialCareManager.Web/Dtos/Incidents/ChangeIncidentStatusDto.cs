namespace SocialCareManager.Web.Dtos;

public class ChangeIncidentStatusDto
{
    public IncidentStatus Status { get; set; }

    public string? Note { get; set; }
}