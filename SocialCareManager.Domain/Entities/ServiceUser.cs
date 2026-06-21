using SocialCareManager.Domain.Common;

namespace SocialCareManager.Domain.Entities;

public class ServiceUser  : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public ICollection<DailyNote> DailyNotes { get; set; }
        = new List<DailyNote>();
}