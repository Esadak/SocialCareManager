namespace SocialCareManager.Web.Dtos;

public class NextOfKinDto
{
    public Guid Id { get; set; }
    public Guid ServiceUserId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public bool IsEmergencyContact { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}