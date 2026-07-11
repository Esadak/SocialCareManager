using SocialCareManager.Domain.Enums;

namespace SocialCareManager.Api.Dtos.MedicationAdministration;

public class MedicationAdministrationDto
{
    public Guid Id { get; set; }
    public Guid ServiceUserId { get; set; }
    public Guid MedicationId { get; set; }

    public string MedicationName { get; set; } = string.Empty;
    public string MedicationStrength { get; set; } = string.Empty;
    public string MedicationDosage { get; set; } = string.Empty;

    public DateTime ScheduledAt { get; set; }
    public DateTime? AdministeredAt { get; set; }

    public MedicationAdministrationStatus Status { get; set; }

    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public string? AdministeredBy { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}