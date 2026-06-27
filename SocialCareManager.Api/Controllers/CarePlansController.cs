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
public class CarePlansController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CarePlansController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CarePlan>>> GetAll(Guid serviceUserId)
    {
        var carePlans = await _context.CarePlans
            .Where(x => x.ServiceUserId == serviceUserId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return Ok(carePlans);
    }

    [HttpGet("active")]
    public async Task<ActionResult<CarePlan>> GetActive(Guid serviceUserId)
    {
        var carePlan = await _context.CarePlans
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
        var serviceUserExists = await _context.ServiceUsers
            .AnyAsync(x => x.Id == serviceUserId);

        if (!serviceUserExists)
            return NotFound("Service user not found.");

        if (dto.IsActive)
        {
            var activePlans = await _context.CarePlans
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

        _context.CarePlans.Add(carePlan);

        await _context.SaveChangesAsync();

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
        var carePlan = await _context.CarePlans
            .FirstOrDefaultAsync(x =>
                x.Id == carePlanId &&
                x.ServiceUserId == serviceUserId);

        if (carePlan is null)
            return NotFound();

        if (updatedCarePlan.IsActive)
        {
            var otherActivePlans = await _context.CarePlans
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

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{carePlanId:guid}")]
    public async Task<IActionResult> Delete(
        Guid serviceUserId,
        Guid carePlanId)
    {
        var carePlan = await _context.CarePlans
            .FirstOrDefaultAsync(x =>
                x.Id == carePlanId &&
                x.ServiceUserId == serviceUserId);

        if (carePlan is null)
            return NotFound();

        _context.CarePlans.Remove(carePlan);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private string GetCurrentUserName()
    {
        var email = User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(email))
            return "Unknown User";

        var user = _context.Users
            .FirstOrDefault(x => x.Email == email);

        if (user is null)
            return email;

        var fullName = $"{user.FirstName} {user.LastName}".Trim();

        return string.IsNullOrWhiteSpace(fullName)
            ? email
            : fullName;
    }
}