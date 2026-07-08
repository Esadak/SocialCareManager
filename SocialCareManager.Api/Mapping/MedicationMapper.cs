using SocialCareManager.Api.Dtos.Medication;
using SocialCareManager.Domain.Entities;

namespace SocialCareManager.Api.Mapping;

public static class MedicationMapper
{
    public static MedicationDto ToDto(this Medication medication)
    {
        return new MedicationDto
        {
            Id = medication.Id,
            ServiceUserId = medication.ServiceUserId,

            Name = medication.Name,
            Strength = medication.Strength,
            Dosage = medication.Dosage,
            Route = medication.Route,
            Frequency = medication.Frequency,
            AdministrationTimes = medication.AdministrationTimes,

            StartDate = medication.StartDate,
            EndDate = medication.EndDate,

            Prescriber = medication.Prescriber,
            Instructions = medication.Instructions,
            IsPrn = medication.IsPrn,
            Reason = medication.Reason,

            IsActive = medication.IsActive,

            CreatedAt = medication.CreatedAt,
            UpdatedAt = medication.UpdatedAt,

            CreatedBy = medication.CreatedBy,
            UpdatedBy = medication.UpdatedBy
        };
    }

    public static Medication ToEntity(
    this CreateMedicationDto dto,
    Guid serviceUserId,
    string createdBy)
{
    return new Medication
    {
        Id = Guid.NewGuid(),

        ServiceUserId = serviceUserId,

        Name = dto.Name,
        Strength = dto.Strength,
        Dosage = dto.Dosage,
        Route = dto.Route,
        Frequency = dto.Frequency,
        AdministrationTimes = dto.AdministrationTimes,

        StartDate = dto.StartDate,
        EndDate = dto.EndDate,

        Prescriber = dto.Prescriber,
        Instructions = dto.Instructions,
        IsPrn = dto.IsPrn,
        Reason = dto.Reason,

        IsActive = dto.IsActive,

        CreatedBy = createdBy,
        UpdatedBy = null
    };
}

public static void UpdateFromDto(
    this Medication medication,
    EditMedicationDto dto,
    string updatedBy)
{
    medication.Name = dto.Name;
    medication.Strength = dto.Strength;
    medication.Dosage = dto.Dosage;
    medication.Route = dto.Route;
    medication.Frequency = dto.Frequency;
    medication.AdministrationTimes = dto.AdministrationTimes;

    medication.StartDate = dto.StartDate;
    medication.EndDate = dto.EndDate;

    medication.Prescriber = dto.Prescriber;
    medication.Instructions = dto.Instructions;
    medication.IsPrn = dto.IsPrn;
    medication.Reason = dto.Reason;
    medication.IsActive = dto.IsActive;

    medication.UpdatedAt = DateTime.UtcNow;
    medication.UpdatedBy = updatedBy;
}
}