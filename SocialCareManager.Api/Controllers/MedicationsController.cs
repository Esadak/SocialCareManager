using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialCareManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SocialCareManager.Api.Dtos.Medication;
using SocialCareManager.Api.Mapping;
using SocialCareManager.Api.Validation;
using SocialCareManager.Web.Dtos;

namespace SocialCareManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/serviceusers/{serviceUserId:guid}/medications")]
public class MedicationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly MedicationValidator _validator;

    public MedicationsController(
    ApplicationDbContext context,
    MedicationValidator validator)
{
    _context = context;
    _validator = validator;
}

   [HttpGet]
public async Task<ActionResult<IEnumerable<MedicationDto>>> GetAll(Guid serviceUserId)
{
    var medications = await _context.Medications
        .Where(x => x.ServiceUserId == serviceUserId)
        .OrderBy(x => x.Name)
        .ToListAsync();

    return Ok(medications.Select(x => x.ToDto()));
}



[HttpGet("{id:guid}")]
public async Task<ActionResult<MedicationDto>> GetById(
    Guid serviceUserId,
    Guid id)
{
    var medication = await _context.Medications
        .FirstOrDefaultAsync(x =>
            x.ServiceUserId == serviceUserId &&
            x.Id == id);

    if (medication is null)
        return NotFound();

    return Ok(medication.ToDto());
}

[HttpPost]
public async Task<ActionResult<MedicationDto>> Create(
    Guid serviceUserId,
    CreateMedicationDto dto)
{
    var serviceUserExists = await _context.ServiceUsers
        .AnyAsync(x => x.Id == serviceUserId);

    if (!serviceUserExists)
        return NotFound("Service user not found.");

    var validationErrors = await _validator.ValidateCreateAsync(serviceUserId, dto);

    if (validationErrors.Count > 0)
        return BadRequest(validationErrors);

    var medication = dto.ToEntity(
        serviceUserId,
        GetCurrentUserName());

    _context.Medications.Add(medication);

    await _context.SaveChangesAsync();

    return CreatedAtAction(
        nameof(GetById),
        new { serviceUserId, id = medication.Id },
        medication.ToDto());
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
