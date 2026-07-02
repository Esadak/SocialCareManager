using SocialCareManager.Domain.Common;

namespace SocialCareManager.Domain.Entities;

public class Medication : BaseEntity
{
    public Guid ServiceUserId { get; set; }

    public ServiceUser ServiceUser { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public string Strength { get; set; } = string.Empty;

    public string Dosage { get; set; } = string.Empty;

    public string Route { get; set; } = string.Empty;

    public string Frequency { get; set; } = string.Empty;

    public string AdministrationTimes { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string Prescriber { get; set; } = string.Empty;

    public string Instructions { get; set; } = string.Empty;

    public bool IsPrn { get; set; }

    public string Reason { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}