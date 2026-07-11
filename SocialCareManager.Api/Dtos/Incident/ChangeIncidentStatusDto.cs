using SocialCareManager.Domain.Enums;

namespace SocialCareManager.Api.Dtos.Incident;

public class ChangeIncidentStatusDto
{
    public IncidentStatus Status { get; set; }

    public string? Note { get; set; }
}