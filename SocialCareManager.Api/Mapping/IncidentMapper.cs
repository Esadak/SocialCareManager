using SocialCareManager.Api.Dtos.Incident;
using SocialCareManager.Domain.Entities;
using SocialCareManager.Domain.Enums;

namespace SocialCareManager.Api.Mapping;

public static class IncidentMapper
{
    public static IncidentDto ToDto(this Incident incident)
    {
        return new IncidentDto
        {
            Id = incident.Id,
            ServiceUserId = incident.ServiceUserId,

            Title = incident.Title,
            Category = incident.Category,
            Severity = incident.Severity,
            Status = incident.Status,
            OccurredAt = incident.OccurredAt,

            Description = incident.Description,
            ImmediateActions = incident.ImmediateActions,
            PeopleInvolved = incident.PeopleInvolved,

            ManagerNotified = incident.ManagerNotified,
            ManagerNotifiedAt = incident.ManagerNotifiedAt,
            ManagerNotifiedBy = incident.ManagerNotifiedBy,

            Outcome = incident.Outcome,
            ClosedAt = incident.ClosedAt,
            ClosedBy = incident.ClosedBy,

            CreatedAt = incident.CreatedAt,
            CreatedBy = incident.CreatedBy,
            UpdatedAt = incident.UpdatedAt,
            UpdatedBy = incident.UpdatedBy,

            FollowUps = incident.FollowUps
                .OrderByDescending(x => x.FollowedUpAt)
                .Select(x => x.ToDto())
                .ToList()
        };
    }

    public static IncidentFollowUpDto ToDto(
        this IncidentFollowUp followUp)
    {
        return new IncidentFollowUpDto
        {
            Id = followUp.Id,
            IncidentId = followUp.IncidentId,
            Note = followUp.Note,
            FollowedUpAt = followUp.FollowedUpAt,
            FollowedUpBy = followUp.FollowedUpBy,
            CreatedAt = followUp.CreatedAt,
            CreatedBy = followUp.CreatedBy
        };
    }

    public static Incident ToEntity(
        this CreateIncidentDto dto,
        Guid serviceUserId,
        string? currentUser)
    {
        var occurredAt = EnsureUtc(dto.OccurredAt);

        return new Incident
        {
            Id = Guid.NewGuid(),
            ServiceUserId = serviceUserId,

            Title = dto.Title.Trim(),
            Category = dto.Category,
            Severity = dto.Severity,
            Status = IncidentStatus.Open,
            OccurredAt = occurredAt,

            Description = dto.Description.Trim(),
            ImmediateActions = dto.ImmediateActions?.Trim(),
            PeopleInvolved = dto.PeopleInvolved?.Trim(),

            ManagerNotified = dto.ManagerNotified,
            ManagerNotifiedAt = dto.ManagerNotified
                ? DateTime.UtcNow
                : null,
            ManagerNotifiedBy = dto.ManagerNotified
                ? currentUser
                : null,

            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUser
        };
    }

    public static void UpdateFromDto(
        this Incident incident,
        EditIncidentDto dto,
        string? currentUser)
    {
        incident.Title = dto.Title.Trim();
        incident.Category = dto.Category;
        incident.Severity = dto.Severity;
        incident.OccurredAt = EnsureUtc(dto.OccurredAt);

        incident.Description = dto.Description.Trim();
        incident.ImmediateActions = dto.ImmediateActions?.Trim();
        incident.PeopleInvolved = dto.PeopleInvolved?.Trim();

        if (!incident.ManagerNotified && dto.ManagerNotified)
        {
            incident.ManagerNotifiedAt = DateTime.UtcNow;
            incident.ManagerNotifiedBy = currentUser;
        }

        if (!dto.ManagerNotified)
        {
            incident.ManagerNotifiedAt = null;
            incident.ManagerNotifiedBy = null;
        }

        incident.ManagerNotified = dto.ManagerNotified;
        incident.UpdatedAt = DateTime.UtcNow;
        incident.UpdatedBy = currentUser;
    }

    public static IncidentFollowUp ToEntity(
        this CreateIncidentFollowUpDto dto,
        Guid incidentId,
        string? currentUser)
    {
        return new IncidentFollowUp
        {
            Id = Guid.NewGuid(),
            IncidentId = incidentId,
            Note = dto.Note.Trim(),
            FollowedUpAt = DateTime.UtcNow,
            FollowedUpBy = currentUser,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUser
        };
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