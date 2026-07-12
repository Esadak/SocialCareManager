using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialCareManager.Api.Dtos.Dashboard;
using SocialCareManager.Domain.Enums;
using SocialCareManager.Infrastructure.Data;

namespace SocialCareManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/dashboard")]
public class DashboardController : BaseApiController
{
    public DashboardController(
        ApplicationDbContext context)
        : base(context)
    {
    }

    [HttpGet("insights")]
    public async Task<ActionResult<DashboardInsightsDto>> GetInsights()
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var tomorrow = today.AddDays(1);
        var dueSoonLimit = today.AddDays(7);

        var totalServiceUsers =
            await Context.ServiceUsers.CountAsync();

        var serviceUsersWithRecentNotes =
            await Context.DailyNotes
                .Where(x => x.CreatedAt >= now.AddHours(-72))
                .Select(x => x.ServiceUserId)
                .Distinct()
                .CountAsync();

        var serviceUsersWithNextOfKin =
            await Context.NextOfKin
                .Select(x => x.ServiceUserId)
                .Distinct()
                .CountAsync();

        var dto = new DashboardInsightsDto
        {
            TotalServiceUsers = totalServiceUsers,

            ActiveCarePlans =
                await Context.CarePlans.CountAsync(x =>
                    x.IsActive),

            CarePlansOverdue =
                await Context.CarePlans.CountAsync(x =>
                    x.IsActive &&
                    x.ReviewDate < today),

            CarePlansDueSoon =
                await Context.CarePlans.CountAsync(x =>
                    x.IsActive &&
                    x.ReviewDate >= today &&
                    x.ReviewDate <= dueSoonLimit),

            OpenIncidents =
                await Context.Incidents.CountAsync(x =>
                    x.Status != IncidentStatus.Closed),

            HighSeverityIncidents =
                await Context.Incidents.CountAsync(x =>
                    x.Status != IncidentStatus.Closed &&
                    (
                        x.Severity == IncidentSeverity.High ||
                        x.Severity == IncidentSeverity.Critical
                    )),

            MedicationWaiting =
                await Context.MedicationAdministrations.CountAsync(x =>
                    x.Status ==
                    MedicationAdministrationStatus.Pending),

            MedicationMissed =
                await Context.MedicationAdministrations.CountAsync(x =>
                    x.Status ==
                        MedicationAdministrationStatus.Refused ||
                    x.Status ==
                        MedicationAdministrationStatus.Omitted ||
                    x.Status ==
                        MedicationAdministrationStatus.NotAvailable),

            CalendarToday =
                await Context.CalendarEvents.CountAsync(x =>
                    x.StartAt >= today &&
                    x.StartAt < tomorrow),

            CalendarOverdue =
                await Context.CalendarEvents.CountAsync(x =>
                    x.Status == CalendarEventStatus.Scheduled &&
                    x.EndAt < now),

            MissingDailyNotes =
                Math.Max(
                    0,
                    totalServiceUsers - serviceUsersWithRecentNotes),

            MissingNextOfKin =
                Math.Max(
                    0,
                    totalServiceUsers - serviceUsersWithNextOfKin)
        };

        return Ok(dto);
    }

    [HttpGet("attention")]
    public async Task<ActionResult<IEnumerable<DashboardAttentionItemDto>>>
        GetAttentionItems([FromQuery] string type)
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var dueSoonLimit = today.AddDays(7);

        var serviceUsers = await Context.ServiceUsers
            .AsNoTracking()
            .ToDictionaryAsync(
                x => x.Id,
                x => $"{x.FirstName} {x.LastName}".Trim());

        List<DashboardAttentionItemDto> result;

        switch (type.Trim().ToLowerInvariant())
        {
            case "careplans-overdue":
                result = await Context.CarePlans
                    .AsNoTracking()
                    .Where(x =>
                        x.IsActive &&
                        x.ReviewDate < today)
                    .OrderBy(x => x.ReviewDate)
                    .Select(x => new DashboardAttentionItemDto
                    {
                        ServiceUserId = x.ServiceUserId,
                        SourceId = x.Id,
                        Title = "Care plan review overdue",
                        Description =
                            $"Version {x.VersionNumber} was due for review.",
                        Category = "Care plan",
                        Severity = "danger",
                        RelevantAt = x.ReviewDate,
                        TargetTab = "careplan"
                    })
                    .ToListAsync();
                break;

            case "careplans-due-soon":
                result = await Context.CarePlans
                    .AsNoTracking()
                    .Where(x =>
                        x.IsActive &&
                        x.ReviewDate >= today &&
                        x.ReviewDate <= dueSoonLimit)
                    .OrderBy(x => x.ReviewDate)
                    .Select(x => new DashboardAttentionItemDto
                    {
                        ServiceUserId = x.ServiceUserId,
                        SourceId = x.Id,
                        Title = "Care plan review due soon",
                        Description =
                            $"Version {x.VersionNumber} requires review.",
                        Category = "Care plan",
                        Severity = "warning",
                        RelevantAt = x.ReviewDate,
                        TargetTab = "careplan"
                    })
                    .ToListAsync();
                break;

            case "medication-waiting":
                result = await Context.MedicationAdministrations
                    .AsNoTracking()
                    .Include(x => x.Medication)
                    .Where(x =>
                        x.Status ==
                        MedicationAdministrationStatus.Pending)
                    .OrderBy(x => x.ScheduledAt)
                    .Select(x => new DashboardAttentionItemDto
                    {
                        ServiceUserId = x.ServiceUserId,
                        SourceId = x.Id,
                        Title = x.Medication.Name,
                        Description = "Medication dose is waiting to be recorded.",
                        Category = "Medication dose",
                        Severity = "danger",
                        RelevantAt = x.ScheduledAt,
                        TargetTab = "medicationdoses"
                    })
                    .ToListAsync();
                break;

            case "medication-missed":
                result = await Context.MedicationAdministrations
                    .AsNoTracking()
                    .Include(x => x.Medication)
                    .Where(x =>
                        x.Status ==
                            MedicationAdministrationStatus.Refused ||
                        x.Status ==
                            MedicationAdministrationStatus.Omitted ||
                        x.Status ==
                            MedicationAdministrationStatus.NotAvailable)
                    .OrderByDescending(x => x.ScheduledAt)
                    .Select(x => new DashboardAttentionItemDto
                    {
                        ServiceUserId = x.ServiceUserId,
                        SourceId = x.Id,
                        Title = x.Medication.Name,
                        Description =
                            x.Reason ?? "Medication was not given.",
                        Category = "Medication dose",
                        Severity = "danger",
                        RelevantAt = x.ScheduledAt,
                        TargetTab = "medicationdoses"
                    })
                    .ToListAsync();
                break;

            case "high-incidents":
                result = await Context.Incidents
                    .AsNoTracking()
                    .Where(x =>
                        x.Status != IncidentStatus.Closed &&
                        (
                            x.Severity == IncidentSeverity.High ||
                            x.Severity == IncidentSeverity.Critical
                        ))
                    .OrderByDescending(x => x.OccurredAt)
                    .Select(x => new DashboardAttentionItemDto
                    {
                        ServiceUserId = x.ServiceUserId,
                        SourceId = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        Category = "Incident",
                        Severity = "danger",
                        RelevantAt = x.OccurredAt,
                        TargetTab = "incidents"
                    })
                    .ToListAsync();
                break;

            case "calendar-overdue":
                result = await Context.CalendarEvents
                    .AsNoTracking()
                    .Where(x =>
                        x.Status == CalendarEventStatus.Scheduled &&
                        x.EndAt < now)
                    .OrderBy(x => x.StartAt)
                    .Select(x => new DashboardAttentionItemDto
                    {
                        ServiceUserId = x.ServiceUserId,
                        SourceId = x.Id,
                        Title = x.Title,
                        Description =
                            x.Description ?? "Scheduled event is overdue.",
                        Category = "Calendar",
                        Severity = "danger",
                        RelevantAt = x.StartAt,
                        TargetTab = "calendar"
                    })
                    .ToListAsync();
                break;

            case "missing-daily-notes":
            {
                var recentServiceUserIds = await Context.DailyNotes
                    .AsNoTracking()
                    .Where(x => x.CreatedAt >= now.AddHours(-72))
                    .Select(x => x.ServiceUserId)
                    .Distinct()
                    .ToListAsync();

                result = await Context.ServiceUsers
                    .AsNoTracking()
                    .Where(x =>
                        !recentServiceUserIds.Contains(x.Id))
                    .OrderBy(x => x.FirstName)
                    .ThenBy(x => x.LastName)
                    .Select(x => new DashboardAttentionItemDto
                    {
                        ServiceUserId = x.Id,
                        Title = "No recent daily note",
                        Description =
                            "No daily note has been recorded in the last 72 hours.",
                        Category = "Daily notes",
                        Severity = "warning",
                        RelevantAt = null,
                        TargetTab = "notes"
                    })
                    .ToListAsync();
                break;
            }

            case "missing-next-of-kin":
            {
                var serviceUserIdsWithContacts =
                    await Context.NextOfKin
                        .AsNoTracking()
                        .Select(x => x.ServiceUserId)
                        .Distinct()
                        .ToListAsync();

                result = await Context.ServiceUsers
                    .AsNoTracking()
                    .Where(x =>
                        !serviceUserIdsWithContacts.Contains(x.Id))
                    .OrderBy(x => x.FirstName)
                    .ThenBy(x => x.LastName)
                    .Select(x => new DashboardAttentionItemDto
                    {
                        ServiceUserId = x.Id,
                        Title = "Next of kin missing",
                        Description =
                            "No next-of-kin contact has been registered.",
                        Category = "Next of kin",
                        Severity = "warning",
                        RelevantAt = null,
                        TargetTab = "nextofkin"
                    })
                    .ToListAsync();
                break;
            }

            default:
                return BadRequest("Unknown dashboard attention type.");
        }

        foreach (var item in result)
        {
            item.ServiceUserName =
                serviceUsers.TryGetValue(
                    item.ServiceUserId,
                    out var name)
                    ? name
                    : "Unknown service user";
        }

        return Ok(result);
    }

    [HttpGet("activity")]
    public async Task<ActionResult<IEnumerable<DashboardActivityDto>>>
        GetActivity([FromQuery] int take = 12)
    {
        take = Math.Clamp(take, 1, 50);

        var serviceUsers = await Context.ServiceUsers
            .AsNoTracking()
            .ToDictionaryAsync(
                x => x.Id,
                x => $"{x.FirstName} {x.LastName}".Trim());

        var dailyNotes = await Context.DailyNotes
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Take(take)
            .Select(x => new DashboardActivityDto
            {
                SourceId = x.Id,
                ServiceUserId = x.ServiceUserId,
                ActivityType = "Daily note",
                Title = x.Title,
                Description = "A daily note was recorded.",
                OccurredAt = x.CreatedAt,
                PerformedBy = x.CreatedBy,
                Icon = "bi bi-journal-text",
                TargetTab = "notes"
            })
            .ToListAsync();

        var carePlans = await Context.CarePlans
            .AsNoTracking()
            .OrderByDescending(x =>
                x.UpdatedAt ?? x.CreatedAt)
            .Take(take)
            .Select(x => new DashboardActivityDto
            {
                SourceId = x.Id,
                ServiceUserId = x.ServiceUserId,
                ActivityType = "Care plan",
                Title = $"Care plan version {x.VersionNumber}",
                Description = x.IsActive
                    ? "Care plan is active."
                    : "Care plan was archived.",
                OccurredAt = x.UpdatedAt ?? x.CreatedAt,
                PerformedBy = x.UpdatedBy ?? x.CreatedBy,
                Icon = "bi bi-clipboard2-check",
                TargetTab = "careplan"
            })
            .ToListAsync();

        var medicationDoses =
            await Context.MedicationAdministrations
                .AsNoTracking()
                .Include(x => x.Medication)
                .OrderByDescending(x =>
                    x.UpdatedAt ?? x.CreatedAt)
                .Take(take)
                .Select(x => new DashboardActivityDto
                {
                    SourceId = x.Id,
                    ServiceUserId = x.ServiceUserId,
                    ActivityType = "Medication dose",
                    Title = x.Medication.Name,
                    Description = x.Status.ToString(),
                    OccurredAt =
                        x.UpdatedAt ?? x.CreatedAt,
                    PerformedBy =
                        x.AdministeredBy ??
                        x.UpdatedBy ??
                        x.CreatedBy,
                    Icon = "bi bi-capsule",
                    TargetTab = "medicationdoses"
                })
                .ToListAsync();

        var incidents = await Context.Incidents
            .AsNoTracking()
            .OrderByDescending(x =>
                x.UpdatedAt ?? x.CreatedAt)
            .Take(take)
            .Select(x => new DashboardActivityDto
            {
                SourceId = x.Id,
                ServiceUserId = x.ServiceUserId,
                ActivityType = "Incident",
                Title = x.Title,
                Description = x.Status.ToString(),
                OccurredAt =
                    x.UpdatedAt ?? x.CreatedAt,
                PerformedBy =
                    x.UpdatedBy ?? x.CreatedBy,
                Icon = "bi bi-exclamation-triangle",
                TargetTab = "incidents"
            })
            .ToListAsync();

        var calendarEvents = await Context.CalendarEvents
            .AsNoTracking()
            .OrderByDescending(x =>
                x.UpdatedAt ?? x.CreatedAt)
            .Take(take)
            .Select(x => new DashboardActivityDto
            {
                SourceId = x.Id,
                ServiceUserId = x.ServiceUserId,
                ActivityType = "Calendar event",
                Title = x.Title,
                Description = x.Status.ToString(),
                OccurredAt =
                    x.UpdatedAt ?? x.CreatedAt,
                PerformedBy =
                    x.UpdatedBy ?? x.CreatedBy,
                Icon = "bi bi-calendar-event",
                TargetTab = "calendar"
            })
            .ToListAsync();

        var result = dailyNotes
            .Concat(carePlans)
            .Concat(medicationDoses)
            .Concat(incidents)
            .Concat(calendarEvents)
            .OrderByDescending(x => x.OccurredAt)
            .Take(take)
            .ToList();

        foreach (var activity in result)
        {
            activity.ServiceUserName =
                serviceUsers.TryGetValue(
                    activity.ServiceUserId,
                    out var name)
                    ? name
                    : "Unknown service user";
        }

        return Ok(result);
    }
}