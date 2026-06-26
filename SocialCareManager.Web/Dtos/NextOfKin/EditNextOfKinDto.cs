namespace SocialCareManager.Web.Dtos;

public class EditNextOfKinDto
{
    public string FullName { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public bool IsEmergencyContact { get; set; }
}