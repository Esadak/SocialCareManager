namespace SocialCareManager.Web.Dtos;

public class EditServiceUserDto
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }
}