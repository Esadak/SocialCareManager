using SocialCareManager.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialCareManager.Domain.Entities;
using SocialCareManager.Infrastructure.Data;


namespace SocialCareManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/serviceusers/{serviceUserId:guid}/careplans")]
public class XController : BaseApiController
{
    public XController(ApplicationDbContext context)
    : base(context)
{
}

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CarePlan>>> GetAll(Guid serviceUserId)
    {
        var carePlans = await Context.CarePlans
            .Where(x => x.ServiceUserId == serviceUserId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return Ok(carePlans);
    }

    [HttpGet("active")]
    public async Task<ActionResult<CarePlan>> GetActive(Guid serviceUserId)
    {
        var carePlan = await Context.CarePlans
            .FirstOrDefaultAsync(x =>
                x.ServiceUserId == serviceUserId &&
                x.IsActive);

        if (carePlan is null)
            return NotFound();

        return Ok(carePlan);
    }

    [HttpPost]
    public async Task<ActionResult<CarePlan>> Create(
    Guid serviceUserId,
    CreateCarePlanDto dto)
    {
        var serviceUserExists = await Context.ServiceUsers
            .AnyAsync(x => x.Id == serviceUserId);

        if (!serviceUserExists)
            return NotFound("Service user not found.");

        if (dto.IsActive)
        {
            var activePlans = await Context.CarePlans
                .Where(x =>
                    x.ServiceUserId == serviceUserId &&
                    x.IsActive)
                .ToListAsync();

            foreach (var activePlan in activePlans)
            {
                activePlan.IsActive = false;
                activePlan.UpdatedAt = DateTime.UtcNow;
                activePlan.UpdatedBy = GetCurrentUserName();
            }
        }

        var carePlan = new CarePlan
{
    Id = Guid.NewGuid(),
    ServiceUserId = serviceUserId,
    Goal = dto.Goal,
    Needs = dto.Needs,
    SupportPlan = dto.SupportPlan,
    RiskAssessment = dto.RiskAssessment,
    ReviewDate = DateTime.SpecifyKind(dto.ReviewDate, DateTimeKind.Utc),
    IsActive = dto.IsActive,
    CreatedBy = GetCurrentUserName(),
    UpdatedBy = null
};

        Context.CarePlans.Add(carePlan);

        await Context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetAll),
            new { serviceUserId },
            carePlan);
    }

    [HttpPut("{carePlanId:guid}")]
    public async Task<IActionResult> Update(
        Guid serviceUserId,
        Guid carePlanId,
        CarePlan updatedCarePlan)
    {
        var carePlan = await Context.CarePlans
            .FirstOrDefaultAsync(x =>
                x.Id == carePlanId &&
                x.ServiceUserId == serviceUserId);

        if (carePlan is null)
            return NotFound();

        if (updatedCarePlan.IsActive)
        {
            var otherActivePlans = await Context.CarePlans
                .Where(x =>
                    x.ServiceUserId == serviceUserId &&
                    x.Id != carePlanId &&
                    x.IsActive)
                .ToListAsync();

            foreach (var activePlan in otherActivePlans)
            {
                activePlan.IsActive = false;
                activePlan.UpdatedAt = DateTime.UtcNow;
                activePlan.UpdatedBy = GetCurrentUserName();
            }
        }

        carePlan.Goal = updatedCarePlan.Goal;
        carePlan.Needs = updatedCarePlan.Needs;
        carePlan.SupportPlan = updatedCarePlan.SupportPlan;
        carePlan.RiskAssessment = updatedCarePlan.RiskAssessment;
        carePlan.ReviewDate = updatedCarePlan.ReviewDate;
        carePlan.IsActive = updatedCarePlan.IsActive;
        carePlan.UpdatedAt = DateTime.UtcNow;
        carePlan.UpdatedBy = GetCurrentUserName();

        await Context.SaveChangesAsync();

        return NoContent();
    }

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

        Context.CarePlans.Remove(carePlan);

        await Context.SaveChangesAsync();

        return NoContent();
    }

  
}