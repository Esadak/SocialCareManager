namespace SocialCareManager.Api.Dtos.MedicationAdministration;

public class EditMedicationAdministrationDto
{
    public DateTime ScheduledAt { get; set; }

    public string? Notes { get; set; }
}