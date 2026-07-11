using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialCareManager.Api.Dtos.Incident;
using SocialCareManager.Api.Mapping;
using SocialCareManager.Api.Validation;
using SocialCareManager.Domain.Enums;
using SocialCareManager.Infrastructure.Data;

namespace SocialCareManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/serviceusers/{serviceUserId:guid}/incidents")]
public class IncidentsController : BaseApiController
{
    private readonly IncidentValidator _validator;

    public IncidentsController(
        ApplicationDbContext context,
        IncidentValidator validator)
        : base(context)
    {
        _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IncidentDto>>> GetAll(
        Guid serviceUserId,
        [FromQuery] IncidentStatus? status = null,
        [FromQuery] IncidentSeverity? severity = null)
    {
        var serviceUserExists = await Context.ServiceUsers
            .AnyAsync(x => x.Id == serviceUserId);

        if (!serviceUserExists)
            return NotFound("Service user not found.");

        var query = Context.Incidents
            .AsNoTracking()
            .Include(x => x.FollowUps)
            .Where(x => x.ServiceUserId == serviceUserId);

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (severity.HasValue)
        {
            query = query.Where(x => x.Severity == severity.Value);
        }

        var incidents = await query
            .OrderByDescending(x => x.OccurredAt)
            .ToListAsync();

        return Ok(incidents.Select(x => x.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<IncidentDto>> GetById(
        Guid serviceUserId,
        Guid id)
    {
        var incident = await Context.Incidents
            .AsNoTracking()
            .Include(x => x.FollowUps)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.ServiceUserId == serviceUserId);

        if (incident is null)
            return NotFound();

        return Ok(incident.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<IncidentDto>> Create(
        Guid serviceUserId,
        CreateIncidentDto dto)
    {
        var serviceUserExists = await Context.ServiceUsers
            .AnyAsync(x => x.Id == serviceUserId);

        if (!serviceUserExists)
            return NotFound("Service user not found.");

        var validationErrors = _validator.ValidateCreate(dto);

        if (validationErrors.Count > 0)
            return BadRequest(validationErrors);

        var incident = dto.ToEntity(
            serviceUserId,
            GetCurrentUserName());

        Context.Incidents.Add(incident);

        await Context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { serviceUserId, id = incident.Id },
            incident.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid serviceUserId,
        Guid id,
        EditIncidentDto dto)
    {
        var incident = await Context.Incidents
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.ServiceUserId == serviceUserId);

        if (incident is null)
            return NotFound();

        var validationErrors =
            _validator.ValidateEdit(incident, dto);

        if (validationErrors.Count > 0)
            return BadRequest(validationErrors);

        incident.UpdateFromDto(
            dto,
            GetCurrentUserName());

        await Context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        Guid serviceUserId,
        Guid id,
        ChangeIncidentStatusDto dto)
    {
        var incident = await Context.Incidents
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.ServiceUserId == serviceUserId);

        if (incident is null)
            return NotFound();

        var validationErrors =
            _validator.ValidateStatusChange(incident, dto);

        if (validationErrors.Count > 0)
            return BadRequest(validationErrors);

        incident.Status = dto.Status;
        incident.UpdatedAt = DateTime.UtcNow;
        incident.UpdatedBy = GetCurrentUserName();

        if (!string.IsNullOrWhiteSpace(dto.Note))
        {
            var followUp = new CreateIncidentFollowUpDto
            {
                Note = dto.Note
            }.ToEntity(
                incident.Id,
                GetCurrentUserName());

            Context.IncidentFollowUps.Add(followUp);
        }

        await Context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:guid}/follow-ups")]
    public async Task<ActionResult<IncidentFollowUpDto>> AddFollowUp(
        Guid serviceUserId,
        Guid id,
        CreateIncidentFollowUpDto dto)
    {
        var incident = await Context.Incidents
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.ServiceUserId == serviceUserId);

        if (incident is null)
            return NotFound();

        var validationErrors =
            _validator.ValidateFollowUp(incident, dto);

        if (validationErrors.Count > 0)
            return BadRequest(validationErrors);

        var followUp = dto.ToEntity(
            incident.Id,
            GetCurrentUserName());

        Context.IncidentFollowUps.Add(followUp);

        incident.UpdatedAt = DateTime.UtcNow;
        incident.UpdatedBy = GetCurrentUserName();

        await Context.SaveChangesAsync();

        return Ok(followUp.ToDto());
    }

    [HttpPost("{id:guid}/close")]
    public async Task<IActionResult> Close(
        Guid serviceUserId,
        Guid id,
        CloseIncidentDto dto)
    {
        var incident = await Context.Incidents
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.ServiceUserId == serviceUserId);

        if (incident is null)
            return NotFound();

        var validationErrors =
            _validator.ValidateClose(incident, dto);

        if (validationErrors.Count > 0)
            return BadRequest(validationErrors);

        incident.Status = IncidentStatus.Closed;
        incident.Outcome = dto.Outcome.Trim();
        incident.ClosedAt = DateTime.UtcNow;
        incident.ClosedBy = GetCurrentUserName();
        incident.UpdatedAt = DateTime.UtcNow;
        incident.UpdatedBy = GetCurrentUserName();

        await Context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid serviceUserId,
        Guid id)
    {
        var incident = await Context.Incidents
            .Include(x => x.FollowUps)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.ServiceUserId == serviceUserId);

        if (incident is null)
            return NotFound();

        incident.MarkAsDeleted(GetCurrentUserName());

        foreach (var followUp in incident.FollowUps)
        {
            followUp.MarkAsDeleted(GetCurrentUserName());
        }

        await Context.SaveChangesAsync();

        return NoContent();
    }
}