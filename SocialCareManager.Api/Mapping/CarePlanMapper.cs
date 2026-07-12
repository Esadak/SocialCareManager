using SocialCareManager.Api.Dtos;
using SocialCareManager.Domain.Entities;

namespace SocialCareManager.Api.Mapping;

public static class CarePlanMapper
{
    public static CarePlanDto ToDto(this CarePlan carePlan)
    {
        return new CarePlanDto
        {
            Id = carePlan.Id,
            ServiceUserId = carePlan.ServiceUserId,

            Goal = carePlan.Goal,
            Needs = carePlan.Needs,
            SupportPlan = carePlan.SupportPlan,
            RiskAssessment = carePlan.RiskAssessment,

            ReviewDate = carePlan.ReviewDate,
            IsActive = carePlan.IsActive,

            VersionNumber = carePlan.VersionNumber,
            PreviousVersionId = carePlan.PreviousVersionId,
            ArchivedAt = carePlan.ArchivedAt,
            ArchivedBy = carePlan.ArchivedBy,

            CreatedAt = carePlan.CreatedAt,
            UpdatedAt = carePlan.UpdatedAt,
            CreatedBy = carePlan.CreatedBy,
            UpdatedBy = carePlan.UpdatedBy
        };
    }

    public static CarePlanHistoryDto ToHistoryDto(this CarePlan carePlan)
    {
        return new CarePlanHistoryDto
        {
            Id = carePlan.Id,
            ServiceUserId = carePlan.ServiceUserId,

            Goal = carePlan.Goal,
            Needs = carePlan.Needs,
            SupportPlan = carePlan.SupportPlan,
            RiskAssessment = carePlan.RiskAssessment,

            ReviewDate = carePlan.ReviewDate,
            IsActive = carePlan.IsActive,

            VersionNumber = carePlan.VersionNumber,
            PreviousVersionId = carePlan.PreviousVersionId,
            ArchivedAt = carePlan.ArchivedAt,
            ArchivedBy = carePlan.ArchivedBy,

            CreatedAt = carePlan.CreatedAt,
            UpdatedAt = carePlan.UpdatedAt,
            CreatedBy = carePlan.CreatedBy,
            UpdatedBy = carePlan.UpdatedBy
        };
    }

    public static CarePlan ToEntity(
        this CreateCarePlanDto dto,
        Guid serviceUserId,
        string? currentUser)
    {
        return new CarePlan
        {
            Id = Guid.NewGuid(),
            ServiceUserId = serviceUserId,

            Goal = dto.Goal.Trim(),
            Needs = dto.Needs.Trim(),
            SupportPlan = dto.SupportPlan.Trim(),
            RiskAssessment = dto.RiskAssessment.Trim(),

            ReviewDate = EnsureUtc(dto.ReviewDate),
            IsActive = true,

            VersionNumber = 1,
            PreviousVersionId = null,
            ArchivedAt = null,
            ArchivedBy = null,

            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUser
        };
    }

    public static CarePlan ToNewVersionEntity(
        this CreateCarePlanDto dto,
        CarePlan previousVersion,
        string? currentUser)
    {
        return new CarePlan
        {
            Id = Guid.NewGuid(),
            ServiceUserId = previousVersion.ServiceUserId,

            Goal = dto.Goal.Trim(),
            Needs = dto.Needs.Trim(),
            SupportPlan = dto.SupportPlan.Trim(),
            RiskAssessment = dto.RiskAssessment.Trim(),

            ReviewDate = EnsureUtc(dto.ReviewDate),
            IsActive = true,

            VersionNumber = previousVersion.VersionNumber + 1,
            PreviousVersionId = previousVersion.Id,
            ArchivedAt = null,
            ArchivedBy = null,

            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUser
        };
    }

    public static void UpdateFromDto(
        this CarePlan carePlan,
        UpdateCarePlanDto dto,
        string? currentUser)
    {
        carePlan.Goal = dto.Goal.Trim();
        carePlan.Needs = dto.Needs.Trim();
        carePlan.SupportPlan = dto.SupportPlan.Trim();
        carePlan.RiskAssessment = dto.RiskAssessment.Trim();

        carePlan.ReviewDate = EnsureUtc(dto.ReviewDate);

        carePlan.UpdatedAt = DateTime.UtcNow;
        carePlan.UpdatedBy = currentUser;
    }

    public static void Archive(
        this CarePlan carePlan,
        string? currentUser)
    {
        carePlan.IsActive = false;
        carePlan.ArchivedAt = DateTime.UtcNow;
        carePlan.ArchivedBy = currentUser;

        carePlan.UpdatedAt = DateTime.UtcNow;
        carePlan.UpdatedBy = currentUser;
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