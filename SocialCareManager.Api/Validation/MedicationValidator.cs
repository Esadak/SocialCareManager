using Microsoft.EntityFrameworkCore;
using SocialCareManager.Api.Dtos.Medication;
using SocialCareManager.Infrastructure.Data;
using SocialCareManager.Web.Dtos;

namespace SocialCareManager.Api.Validation;

public class MedicationValidator
{
    private readonly ApplicationDbContext _context;

    public MedicationValidator(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<string>> ValidateCreateAsync(
        Guid serviceUserId,
        CreateMedicationDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Name))
            errors.Add("Medication name is required.");

        if (string.IsNullOrWhiteSpace(dto.Dosage))
            errors.Add("Dosage is required.");

        if (dto.StartDate == default)
            errors.Add("Start date is required.");

        if (dto.EndDate is not null && dto.EndDate.Value.Date < dto.StartDate.Date)
            errors.Add("End date cannot be before start date.");

        if (!dto.IsPrn && string.IsNullOrWhiteSpace(dto.Frequency))
            errors.Add("Frequency is required for non-PRN medication.");

        var duplicateActiveMedicationExists = await _context.Medications
            .AnyAsync(x =>
                x.ServiceUserId == serviceUserId &&
                x.IsActive &&
                x.Name.ToLower() == dto.Name.ToLower());

        if (dto.IsActive && duplicateActiveMedicationExists)
            errors.Add("An active medication with the same name already exists for this service user.");

        return errors;
    }


    public async Task<List<string>> ValidateUpdateAsync(
    Guid serviceUserId,
    Guid medicationId,
    EditMedicationDto dto)
{
    var errors = new List<string>();

    if (string.IsNullOrWhiteSpace(dto.Name))
        errors.Add("Medication name is required.");

    if (string.IsNullOrWhiteSpace(dto.Dosage))
        errors.Add("Dosage is required.");

    if (dto.StartDate == default)
        errors.Add("Start date is required.");

    if (dto.EndDate is not null &&
        dto.EndDate.Value.Date < dto.StartDate.Date)
    {
        errors.Add("End date cannot be before start date.");
    }

    if (!dto.IsPrn &&
        string.IsNullOrWhiteSpace(dto.Frequency))
    {
        errors.Add("Frequency is required for non-PRN medication.");
    }

    var duplicateExists = await _context.Medications
        .AnyAsync(x =>
            x.ServiceUserId == serviceUserId &&
            x.Id != medicationId &&
            x.IsActive &&
            x.Name.ToLower() == dto.Name.ToLower());

    if (dto.IsActive && duplicateExists)
    {
        errors.Add("Another active medication with the same name already exists.");
    }

    return errors;
}
}