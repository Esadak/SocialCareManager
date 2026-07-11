namespace SocialCareManager.Web.Dtos;

public class CreateMedicationAdministrationDto
{
    public Guid MedicationId { get; set; }

    public DateTime ScheduledAt { get; set; } = DateTime.Now;

    public string? Notes { get; set; }
}