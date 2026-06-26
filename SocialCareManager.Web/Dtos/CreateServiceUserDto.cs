namespace SocialCareManager.Web.Dtos;

public class CreateServiceUserDto
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; } = DateTime.Today;
}