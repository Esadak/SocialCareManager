using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialCareManager.Api.Dtos.Calendar;
using SocialCareManager.Api.Mapping;
using SocialCareManager.Api.Validation;
using SocialCareManager.Domain.Enums;
using SocialCareManager.Infrastructure.Data;

namespace SocialCareManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/serviceusers/{serviceUserId:guid}/calendar-events")]
public class CalendarEventsController : BaseApiController
{
    private readonly CalendarEventValidator _validator;

    public CalendarEventsController(
        ApplicationDbContext context,
        CalendarEventValidator validator)
        : base(context)
    {
        _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CalendarEventDto>>> GetAll(
        Guid serviceUserId,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] CalendarEventStatus? status = null,
        [FromQuery] CalendarEventType? eventType = null)
    {
        var serviceUserExists = await Context.ServiceUsers
            .AnyAsync(x => x.Id == serviceUserId);

        if (!serviceUserExists)
            return NotFound("Service user not found.");

        var query = Context.CalendarEvents
            .AsNoTracking()
            .Where(x => x.ServiceUserId == serviceUserId);

        if (from.HasValue)
        {
            var fromUtc = EnsureUtc(from.Value);

            query = query.Where(x =>
                x.EndAt >= fromUtc);
        }

        if (to.HasValue)
        {
            var toUtc = EnsureUtc(to.Value);

            query = query.Where(x =>
                x.StartAt <= toUtc);
        }

        if (status.HasValue)
        {
            query = query.Where(x =>
                x.Status == status.Value);
        }

        if (eventType.HasValue)
        {
            query = query.Where(x =>
                x.EventType == eventType.Value);
        }

        var events = await query
            .OrderBy(x => x.StartAt)
            .ToListAsync();

        return Ok(events.Select(x => x.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CalendarEventDto>> GetById(
        Guid serviceUserId,
        Guid id)
    {
        var calendarEvent = await Context.CalendarEvents
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.ServiceUserId == serviceUserId);

        if (calendarEvent is null)
            return NotFound();

        return Ok(calendarEvent.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<CalendarEventDto>> Create(
        Guid serviceUserId,
        CreateCalendarEventDto dto)
    {
        var serviceUserExists = await Context.ServiceUsers
            .AnyAsync(x => x.Id == serviceUserId);

        if (!serviceUserExists)
            return NotFound("Service user not found.");

        var validationErrors =
            _validator.ValidateCreate(dto);

        if (validationErrors.Count > 0)
            return BadRequest(validationErrors);

        var calendarEvent = dto.ToEntity(
            serviceUserId,
            GetCurrentUserName());

        Context.CalendarEvents.Add(calendarEvent);

        await Context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                serviceUserId,
                id = calendarEvent.Id
            },
            calendarEvent.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid serviceUserId,
        Guid id,
        EditCalendarEventDto dto)
    {
        var calendarEvent = await Context.CalendarEvents
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.ServiceUserId == serviceUserId);

        if (calendarEvent is null)
            return NotFound();

        var validationErrors =
            _validator.ValidateEdit(calendarEvent, dto);

        if (validationErrors.Count > 0)
            return BadRequest(validationErrors);

        calendarEvent.UpdateFromDto(
            dto,
            GetCurrentUserName());

        await Context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(
        Guid serviceUserId,
        Guid id,
        CompleteCalendarEventDto dto)
    {
        var calendarEvent = await Context.CalendarEvents
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.ServiceUserId == serviceUserId);

        if (calendarEvent is null)
            return NotFound();

        var validationErrors =
            _validator.ValidateComplete(calendarEvent);

        if (validationErrors.Count > 0)
            return BadRequest(validationErrors);

        calendarEvent.Complete(
            GetCurrentUserName());

        await Context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid serviceUserId,
        Guid id,
        CancelCalendarEventDto dto)
    {
        var calendarEvent = await Context.CalendarEvents
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.ServiceUserId == serviceUserId);

        if (calendarEvent is null)
            return NotFound();

        var validationErrors =
            _validator.ValidateCancel(
                calendarEvent,
                dto);

        if (validationErrors.Count > 0)
            return BadRequest(validationErrors);

        calendarEvent.Cancel(
            dto.Reason,
            GetCurrentUserName());

        await Context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:guid}/reopen")]
    public async Task<IActionResult> Reopen(
        Guid serviceUserId,
        Guid id,
        ReopenCalendarEventDto dto)
    {
        var calendarEvent = await Context.CalendarEvents
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.ServiceUserId == serviceUserId);

        if (calendarEvent is null)
            return NotFound();

        var validationErrors =
            _validator.ValidateReopen(
                calendarEvent,
                dto);

        if (validationErrors.Count > 0)
            return BadRequest(validationErrors);

        calendarEvent.Reopen(
            GetCurrentUserName());

        await Context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid serviceUserId,
        Guid id)
    {
        var calendarEvent = await Context.CalendarEvents
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.ServiceUserId == serviceUserId);

        if (calendarEvent is null)
            return NotFound();

        calendarEvent.MarkAsDeleted(
            GetCurrentUserName());

        await Context.SaveChangesAsync();

        return NoContent();
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(
                value,
                DateTimeKind.Utc)
        };
    }
}