namespace SocialCareManager.Web.Dtos;

public class RecordMedicationAdministrationDto
{
    public MedicationAdministrationStatus Status { get; set; }

    public DateTime? AdministeredAt { get; set; }

    public string? Reason { get; set; }

    public string? Notes { get; set; }
}