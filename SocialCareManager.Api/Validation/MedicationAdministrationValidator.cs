using Microsoft.EntityFrameworkCore;
using SocialCareManager.Api.Dtos.MedicationAdministration;
using SocialCareManager.Domain.Enums;
using SocialCareManager.Infrastructure.Data;

namespace SocialCareManager.Api.Validation;

public class MedicationAdministrationValidator
{
    private readonly ApplicationDbContext _context;

    public MedicationAdministrationValidator(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<string>> ValidateCreateAsync(
        Guid serviceUserId,
        CreateMedicationAdministrationDto dto)
    {
        var errors = new List<string>();

        if (dto.MedicationId == Guid.Empty)
        {
            errors.Add("Select a medication.");
        }

        if (dto.ScheduledAt == default)
        {
            errors.Add("Enter when the medication should be given.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Notes) &&
            dto.Notes.Length > 2000)
        {
            errors.Add("Notes cannot exceed 2000 characters.");
        }

        if (errors.Count > 0)
            return errors;

        var medicationExists = await _context.Medications
            .AnyAsync(x =>
                x.Id == dto.MedicationId &&
                x.ServiceUserId == serviceUserId);

        if (!medicationExists)
        {
            errors.Add("The selected medication could not be found.");
            return errors;
        }

        var scheduledAtUtc = EnsureUtc(dto.ScheduledAt);

        var duplicateExists = await _context.MedicationAdministrations
            .AnyAsync(x =>
                x.ServiceUserId == serviceUserId &&
                x.MedicationId == dto.MedicationId &&
                x.ScheduledAt == scheduledAtUtc);

        if (duplicateExists)
        {
            errors.Add(
                "This medication is already scheduled for the selected time.");
        }

        return errors;
    }

    public List<string> ValidateEdit(
        EditMedicationAdministrationDto dto)
    {
        var errors = new List<string>();

        if (dto.ScheduledAt == default)
        {
            errors.Add("Enter when the medication should be given.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Notes) &&
            dto.Notes.Length > 2000)
        {
            errors.Add("Notes cannot exceed 2000 characters.");
        }

        return errors;
    }

    public List<string> ValidateRecord(
        RecordMedicationAdministrationDto dto)
    {
        var errors = new List<string>();

        if (dto.Status == MedicationAdministrationStatus.Pending)
        {
            errors.Add(
                "Select what happened when the medication was due.");
        }

        var reasonRequired =
            dto.Status == MedicationAdministrationStatus.Refused ||
            dto.Status == MedicationAdministrationStatus.NotAvailable ||
            dto.Status == MedicationAdministrationStatus.Omitted ||
            dto.Status == MedicationAdministrationStatus.Delayed;

        if (reasonRequired && string.IsNullOrWhiteSpace(dto.Reason))
        {
            errors.Add("Enter a reason for this status.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Reason) &&
            dto.Reason.Length > 500)
        {
            errors.Add("Reason cannot exceed 500 characters.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Notes) &&
            dto.Notes.Length > 2000)
        {
            errors.Add("Notes cannot exceed 2000 characters.");
        }

        return errors;
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