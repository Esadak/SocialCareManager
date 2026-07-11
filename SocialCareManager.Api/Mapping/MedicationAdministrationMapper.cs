using SocialCareManager.Api.Dtos.MedicationAdministration;
using SocialCareManager.Domain.Entities;
using SocialCareManager.Domain.Enums;

namespace SocialCareManager.Api.Mapping;

public static class MedicationAdministrationMapper
{
    public static MedicationAdministrationDto ToDto(
        this MedicationAdministration administration)
    {
        return new MedicationAdministrationDto
        {
            Id = administration.Id,
            ServiceUserId = administration.ServiceUserId,
            MedicationId = administration.MedicationId,

            MedicationName = administration.Medication?.Name ?? string.Empty,
            MedicationStrength = administration.Medication?.Strength ?? string.Empty,
            MedicationDosage = administration.Medication?.Dosage ?? string.Empty,

            ScheduledAt = administration.ScheduledAt,
            AdministeredAt = administration.AdministeredAt,
            Status = administration.Status,

            Reason = administration.Reason,
            Notes = administration.Notes,
            AdministeredBy = administration.AdministeredBy,

            CreatedAt = administration.CreatedAt,
            CreatedBy = administration.CreatedBy,
            UpdatedAt = administration.UpdatedAt,
            UpdatedBy = administration.UpdatedBy
        };
    }

    public static MedicationAdministration ToEntity(
        this CreateMedicationAdministrationDto dto,
        Guid serviceUserId,
        string? currentUser)
    {
        return new MedicationAdministration
        {
            Id = Guid.NewGuid(),
            ServiceUserId = serviceUserId,
            MedicationId = dto.MedicationId,
            ScheduledAt = EnsureUtc(dto.ScheduledAt),

            Status = MedicationAdministrationStatus.Pending,
            Notes = dto.Notes?.Trim(),

            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUser
        };
    }

    public static void UpdateFromDto(
        this MedicationAdministration administration,
        EditMedicationAdministrationDto dto,
        string? currentUser)
    {
        administration.ScheduledAt = EnsureUtc(dto.ScheduledAt);
        administration.Notes = dto.Notes?.Trim();

        administration.UpdatedAt = DateTime.UtcNow;
        administration.UpdatedBy = currentUser;
    }

    public static void RecordFromDto(
        this MedicationAdministration administration,
        RecordMedicationAdministrationDto dto,
        string? currentUser)
    {
        administration.Status = dto.Status;

        administration.AdministeredAt = dto.AdministeredAt.HasValue
            ? EnsureUtc(dto.AdministeredAt.Value)
            : DateTime.UtcNow;

        administration.Reason = dto.Reason?.Trim();

        if (!string.IsNullOrWhiteSpace(dto.Notes))
        {
            administration.Notes = dto.Notes.Trim();
        }

        administration.AdministeredBy = currentUser;
        administration.UpdatedAt = DateTime.UtcNow;
        administration.UpdatedBy = currentUser;
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}