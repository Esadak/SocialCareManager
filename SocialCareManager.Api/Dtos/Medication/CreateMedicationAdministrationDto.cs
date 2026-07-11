namespace SocialCareManager.Api.Dtos.MedicationAdministration;

public class CreateMedicationAdministrationDto
{
    public Guid MedicationId { get; set; }

    public DateTime ScheduledAt { get; set; }

    public string? Notes { get; set; }
}