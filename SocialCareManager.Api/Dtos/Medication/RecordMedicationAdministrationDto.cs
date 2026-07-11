using SocialCareManager.Domain.Enums;

namespace SocialCareManager.Api.Dtos.MedicationAdministration;

public class RecordMedicationAdministrationDto
{
    public MedicationAdministrationStatus Status { get; set; }

    public DateTime? AdministeredAt { get; set; }

    public string? Reason { get; set; }

    public string? Notes { get; set; }
}