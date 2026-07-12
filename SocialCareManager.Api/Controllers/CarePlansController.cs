using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialCareManager.Api.Dtos;
using SocialCareManager.Api.Mapping;
using SocialCareManager.Api.Validation;
using SocialCareManager.Infrastructure.Data;

namespace SocialCareManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/serviceusers/{serviceUserId:guid}/careplans")]
public class CarePlansController : BaseApiController
{
    private readonly CarePlanValidator _validator;

    public CarePlansController(
        ApplicationDbContext context,
        CarePlanValidator validator)
        : base(context)
    {
        _validator = validator;
    }

    // Hämta aktiv plan
    [HttpGet("active")]
    public async Task<ActionResult<CarePlanDto>> GetActive(
        Guid serviceUserId)
    {
        var carePlan = await Context.CarePlans
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.ServiceUserId == serviceUserId &&
                x.IsActive);

        if (carePlan is null)
            return NotFound();

        return Ok(carePlan.ToDto());
    }

    // Hämta alla versioner
    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<CarePlanHistoryDto>>> GetHistory(
        Guid serviceUserId)
    {
        var serviceUserExists = await Context.ServiceUsers
            .AnyAsync(x => x.Id == serviceUserId);

        if (!serviceUserExists)
            return NotFound("Service user not found.");

        var history = await Context.CarePlans
            .AsNoTracking()
            .Where(x => x.ServiceUserId == serviceUserId)
            .OrderByDescending(x => x.VersionNumber)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync();

        return Ok(history.Select(x => x.ToHistoryDto()));
    }

    // Hämta en viss version
    [HttpGet("{carePlanId:guid}")]
    public async Task<ActionResult<CarePlanDto>> GetById(
        Guid serviceUserId,
        Guid carePlanId)
    {
        var carePlan = await Context.CarePlans
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == carePlanId &&
                x.ServiceUserId == serviceUserId);

        if (carePlan is null)
            return NotFound();

        return Ok(carePlan.ToDto());
    }

    // Skapa första planen
    [HttpPost]
    public async Task<ActionResult<CarePlanDto>> Create(
        Guid serviceUserId,
        CreateCarePlanDto dto)
    {
        var serviceUserExists = await Context.ServiceUsers
            .AnyAsync(x => x.Id == serviceUserId);

        if (!serviceUserExists)
            return NotFound("Service user not found.");

        var activePlanExists = await Context.CarePlans
            .AnyAsync(x =>
                x.ServiceUserId == serviceUserId &&
                x.IsActive);

        if (activePlanExists)
        {
            return BadRequest(
                "This person already has an active care plan. Update it or create a new version.");
        }

        var validationErrors = _validator.ValidateCreate(dto);

        if (validationErrors.Count > 0)
            return BadRequest(validationErrors);

        var carePlan = dto.ToEntity(
            serviceUserId,
            GetCurrentUserName());

        Context.CarePlans.Add(carePlan);

        await Context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                serviceUserId,
                carePlanId = carePlan.Id
            },
            carePlan.ToDto());
    }

    // Uppdatera nuvarande aktiva plan
    [HttpPut("{carePlanId:guid}")]
    public async Task<IActionResult> Update(
        Guid serviceUserId,
        Guid carePlanId,
        UpdateCarePlanDto dto)
    {
        var carePlan = await Context.CarePlans
            .FirstOrDefaultAsync(x =>
                x.Id == carePlanId &&
                x.ServiceUserId == serviceUserId);

        if (carePlan is null)
            return NotFound();

        var validationErrors =
            _validator.ValidateUpdate(carePlan, dto);

        if (validationErrors.Count > 0)
            return BadRequest(validationErrors);

        carePlan.UpdateFromDto(
            dto,
            GetCurrentUserName());

        await Context.SaveChangesAsync();

        return NoContent();
    }

    // Skapa ny version och arkivera den gamla
    [HttpPost("{carePlanId:guid}/new-version")]
    public async Task<ActionResult<CarePlanDto>> CreateNewVersion(
        Guid serviceUserId,
        Guid carePlanId,
        CreateCarePlanDto dto)
    {
        await using var transaction =
            await Context.Database.BeginTransactionAsync();

        var currentPlan = await Context.CarePlans
            .FirstOrDefaultAsync(x =>
                x.Id == carePlanId &&
                x.ServiceUserId == serviceUserId);

        if (currentPlan is null)
            return NotFound();

        var validationErrors =
            _validator.ValidateCreateNewVersion(currentPlan, dto);

        if (validationErrors.Count > 0)
            return BadRequest(validationErrors);

        var otherActivePlans = await Context.CarePlans
            .Where(x =>
                x.ServiceUserId == serviceUserId &&
                x.IsActive &&
                x.Id != currentPlan.Id)
            .ToListAsync();

        foreach (var activePlan in otherActivePlans)
        {
            activePlan.Archive(GetCurrentUserName());
        }

        currentPlan.Archive(GetCurrentUserName());

        var newVersion = dto.ToNewVersionEntity(
            currentPlan,
            GetCurrentUserName());

        Context.CarePlans.Add(newVersion);

        await Context.SaveChangesAsync();
        await transaction.CommitAsync();

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                serviceUserId,
                carePlanId = newVersion.Id
            },
            newVersion.ToDto());
    }

    // Soft delete, endast Admin
    [Authorize(Roles = "Admin")]
    [HttpDelete("{carePlanId:guid}")]
    public async Task<IActionResult> Delete(
        Guid serviceUserId,
        Guid carePlanId)
    {
        var carePlan = await Context.CarePlans
            .FirstOrDefaultAsync(x =>
                x.Id == carePlanId &&
                x.ServiceUserId == serviceUserId);

        if (carePlan is null)
            return NotFound();

        carePlan.MarkAsDeleted(GetCurrentUserName());

        await Context.SaveChangesAsync();

        return NoContent();
    }
}