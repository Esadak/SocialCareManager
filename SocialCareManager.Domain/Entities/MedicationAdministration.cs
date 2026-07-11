using SocialCareManager.Domain.Common;
using SocialCareManager.Domain.Enums;

namespace SocialCareManager.Domain.Entities;

public class MedicationAdministration : BaseEntity
{
    public Guid ServiceUserId { get; set; }

    public Guid MedicationId { get; set; }

    /// <summary>
    /// The date and time when the medication is scheduled to be given.
    /// Stored as UTC.
    /// </summary>
    public DateTime ScheduledAt { get; set; }

    /// <summary>
    /// The actual date and time when the medication was administered.
    /// Stored as UTC.
    /// </summary>
    public DateTime? AdministeredAt { get; set; }

    public MedicationAdministrationStatus Status { get; set; }
        = MedicationAdministrationStatus.Pending;

    /// <summary>
    /// Reason when medication was refused, omitted, delayed
    /// or unavailable.
    /// </summary>
    public string? Reason { get; set; }

    public string? Notes { get; set; }

    /// <summary>
    /// The user who signed the administration.
    /// This is separate from CreatedBy because a record may be
    /// created before another staff member administers the medication.
    /// </summary>
    public string? AdministeredBy { get; set; }

    public ServiceUser ServiceUser { get; set; } = null!;

    public Medication Medication { get; set; } = null!;
}