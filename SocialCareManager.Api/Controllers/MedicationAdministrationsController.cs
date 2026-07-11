using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialCareManager.Api.Dtos.MedicationAdministration;
using SocialCareManager.Api.Mapping;
using SocialCareManager.Api.Validation;
using SocialCareManager.Domain.Enums;
using SocialCareManager.Infrastructure.Data;

namespace SocialCareManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/serviceusers/{serviceUserId:guid}/medication-administrations")]
public class MedicationAdministrationsController : BaseApiController
{
    private readonly MedicationAdministrationValidator _validator;

    public MedicationAdministrationsController(
        ApplicationDbContext context,
        MedicationAdministrationValidator validator)
        : base(context)
    {
        _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<
        IEnumerable<MedicationAdministrationDto>>> GetAll(
        Guid serviceUserId)
    {
        var serviceUserExists = await Context.ServiceUsers
            .AnyAsync(x => x.Id == serviceUserId);

        if (!serviceUserExists)
            return NotFound("Service user not found.");

        var administrations = await Context.MedicationAdministrations
            .AsNoTracking()
            .Include(x => x.Medication)
            .Where(x => x.ServiceUserId == serviceUserId)
            .OrderByDescending(x => x.ScheduledAt)
            .ToListAsync();

        return Ok(administrations.Select(x => x.ToDto()));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MedicationAdministrationDto>> GetById(
        Guid serviceUserId,
        Guid id)
    {
        var administration = await Context.MedicationAdministrations
            .AsNoTracking()
            .Include(x => x.Medication)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.ServiceUserId == serviceUserId);

        if (administration is null)
            return NotFound();

        return Ok(administration.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<MedicationAdministrationDto>> Create(
        Guid serviceUserId,
        CreateMedicationAdministrationDto dto)
    {
        var serviceUserExists = await Context.ServiceUsers
            .AnyAsync(x => x.Id == serviceUserId);

        if (!serviceUserExists)
            return NotFound("Service user not found.");

        var validationErrors =
            await _validator.ValidateCreateAsync(serviceUserId, dto);

        if (validationErrors.Count > 0)
            return BadRequest(validationErrors);

        var administration = dto.ToEntity(
            serviceUserId,
            GetCurrentUserName());

        Context.MedicationAdministrations.Add(administration);

        await Context.SaveChangesAsync();

        var created = await Context.MedicationAdministrations
            .AsNoTracking()
            .Include(x => x.Medication)
            .FirstAsync(x => x.Id == administration.Id);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                serviceUserId,
                id = administration.Id
            },
            created.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid serviceUserId,
        Guid id,
        EditMedicationAdministrationDto dto)
    {
        var administration = await Context.MedicationAdministrations
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.ServiceUserId == serviceUserId);

        if (administration is null)
            return NotFound();

        if (administration.Status != MedicationAdministrationStatus.Pending)
        {
            return BadRequest(
                "A completed medication record cannot be rescheduled.");
        }

        var validationErrors = _validator.ValidateEdit(dto);

        if (validationErrors.Count > 0)
            return BadRequest(validationErrors);

        administration.UpdateFromDto(
            dto,
            GetCurrentUserName());

        await Context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:guid}/record")]
    public async Task<IActionResult> RecordAdministration(
        Guid serviceUserId,
        Guid id,
        RecordMedicationAdministrationDto dto)
    {
        var administration = await Context.MedicationAdministrations
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.ServiceUserId == serviceUserId);

        if (administration is null)
            return NotFound();

        if (administration.Status != MedicationAdministrationStatus.Pending)
        {
            return BadRequest(
                "This medication administration has already been recorded.");
        }

        var validationErrors = _validator.ValidateRecord(dto);

        if (validationErrors.Count > 0)
            return BadRequest(validationErrors);

        administration.RecordFromDto(
            dto,
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
        var administration = await Context.MedicationAdministrations
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.ServiceUserId == serviceUserId);

        if (administration is null)
            return NotFound();

        administration.MarkAsDeleted(GetCurrentUserName());

        await Context.SaveChangesAsync();

        return NoContent();
    }
} 