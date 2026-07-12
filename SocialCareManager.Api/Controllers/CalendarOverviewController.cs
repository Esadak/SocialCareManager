using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialCareManager.Api.Dtos.CalendarOverview;
using SocialCareManager.Domain.Enums;
using SocialCareManager.Infrastructure.Data;

namespace SocialCareManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/serviceusers/{serviceUserId:guid}/calendar-overview")]
public class CalendarOverviewController : BaseApiController
{
    public CalendarOverviewController(ApplicationDbContext context)
        : base(context)
    {
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CalendarOverviewItemDto>>> GetAll(
        Guid serviceUserId,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var serviceUserExists = await Context.ServiceUsers
            .AnyAsync(x => x.Id == serviceUserId);

        if (!serviceUserExists)
            return NotFound("Service user not found.");

        var fromUtc = from.HasValue
            ? EnsureUtc(from.Value)
            : DateTime.UtcNow.AddDays(-7);

        var toUtc = to.HasValue
            ? EnsureUtc(to.Value)
            : DateTime.UtcNow.AddMonths(1);

        if (toUtc < fromUtc)
        {
            return BadRequest(
                "The end of the date range must be after the start.");
        }

        var calendarEvents = await Context.CalendarEvents
            .AsNoTracking()
            .Where(x =>
                x.ServiceUserId == serviceUserId &&
                x.EndAt >= fromUtc &&
                x.StartAt <= toUtc)
            .Select(x => new CalendarOverviewItemDto
            {
                SourceId = x.Id,
                ServiceUserId = x.ServiceUserId,
                Source = CalendarItemSource.CalendarEvent,

                Title = x.Title,
                TypeText = GetCalendarEventTypeText(x.EventType),
                StatusText = GetCalendarEventStatusText(x.Status),

                StartAt = x.StartAt,
                EndAt = x.EndAt,
                IsAllDay = x.IsAllDay,

                Location = x.Location,
                Description = x.Description,
                ResponsiblePerson = x.AssignedTo,

                RequiresAttention =
                    x.Status == CalendarEventStatus.Scheduled &&
                    x.EndAt < DateTime.UtcNow
            })
            .ToListAsync();

        var medicationDoses = await Context.MedicationAdministrations
            .AsNoTracking()
            .Include(x => x.Medication)
            .Where(x =>
                x.ServiceUserId == serviceUserId &&
                x.ScheduledAt >= fromUtc &&
                x.ScheduledAt <= toUtc)
            .Select(x => new CalendarOverviewItemDto
            {
                SourceId = x.Id,
                ServiceUserId = x.ServiceUserId,
                Source = CalendarItemSource.MedicationDose,

                Title =
                    x.Medication.Name +
                    (string.IsNullOrWhiteSpace(x.Medication.Strength)
                        ? string.Empty
                        : $" {x.Medication.Strength}"),

                TypeText = "Medication dose",
                StatusText = GetMedicationStatusText(x.Status),

                StartAt = x.ScheduledAt,
                EndAt = null,
                IsAllDay = false,

                Location = null,
                Description = x.Notes,
                ResponsiblePerson = x.AdministeredBy,

                RequiresAttention =
                    x.Status == MedicationAdministrationStatus.Pending &&
                    x.ScheduledAt < DateTime.UtcNow
            })
            .ToListAsync();
        
        var carePlanReviews = await Context.CarePlans
    .AsNoTracking()
    .Where(x =>
        x.ServiceUserId == serviceUserId &&
        x.IsActive &&
        x.ReviewDate >= fromUtc &&
        x.ReviewDate <= toUtc)
    .Select(x => new CalendarOverviewItemDto
    {
        SourceId = x.Id,
        ServiceUserId = x.ServiceUserId,
        Source = CalendarItemSource.CarePlanReview,

        Title = "Care plan review",
        TypeText = "Care plan review",
        StatusText = x.ReviewDate < DateTime.UtcNow
            ? "Overdue"
            : "Scheduled",

        StartAt = x.ReviewDate,
        EndAt = null,
        IsAllDay = true,

        Location = null,
        Description = x.Goal,
        ResponsiblePerson = null,

        RequiresAttention =
            x.ReviewDate < DateTime.UtcNow
    })
    .ToListAsync();

        var result = calendarEvents
          .Concat(medicationDoses)
          .Concat(carePlanReviews)
          .OrderBy(x => x.StartAt)
          .ThenBy(x => x.Title)
          .ToList();

        return Ok(result);
    }

    private static string GetCalendarEventTypeText(
        CalendarEventType eventType)
    {
        return eventType switch
        {
            CalendarEventType.DoctorVisit => "Doctor visit",
            CalendarEventType.DentistVisit => "Dentist visit",
            CalendarEventType.SipMeeting => "SIP meeting",
            CalendarEventType.CarePlanReview => "Care plan review",
            CalendarEventType.HomeVisit => "Home visit",
            _ => eventType.ToString()
        };
    }

    private static string GetCalendarEventStatusText(
        CalendarEventStatus status)
    {
        return status switch
        {
            CalendarEventStatus.Scheduled => "Scheduled",
            CalendarEventStatus.Completed => "Completed",
            CalendarEventStatus.Cancelled => "Cancelled",
            _ => status.ToString()
        };
    }

    private static string GetMedicationStatusText(
        MedicationAdministrationStatus status)
    {
        return status switch
        {
            MedicationAdministrationStatus.Pending => "Waiting",
            MedicationAdministrationStatus.Given => "Given",
            MedicationAdministrationStatus.Refused => "Refused",
            MedicationAdministrationStatus.NotAvailable => "Not available",
            MedicationAdministrationStatus.Omitted => "Not given",
            MedicationAdministrationStatus.Delayed => "Delayed",
            _ => status.ToString()
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