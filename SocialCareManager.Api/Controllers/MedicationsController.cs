using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialCareManager.Api.Dtos.Medication;
using SocialCareManager.Api.Mapping;
using SocialCareManager.Api.Validation;
using SocialCareManager.Infrastructure.Data;
using SocialCareManager.Web.Dtos;

namespace SocialCareManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/serviceusers/{serviceUserId:guid}/medications")]
public class MedicationsController : BaseApiController
{
    private readonly MedicationValidator _validator;

    public MedicationsController(
        ApplicationDbContext context,
        MedicationValidator validator)
        : base(context)
    {
        _validator = validator;
    }

   [HttpGet]
public async Task<ActionResult<IEnumerable<MedicationDto>>> GetAll(Guid serviceUserId)
{
    var medications = await Context.Medications
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
    var medication = await Context.Medications
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
    var serviceUserExists = await Context.ServiceUsers
        .AnyAsync(x => x.Id == serviceUserId);

    if (!serviceUserExists)
        return NotFound("Service user not found.");

    var validationErrors = await _validator.ValidateCreateAsync(serviceUserId, dto);

    if (validationErrors.Count > 0)
        return BadRequest(validationErrors);

    var medication = dto.ToEntity(
        serviceUserId,
        GetCurrentUserName());

    Context.Medications.Add(medication);

    await Context.SaveChangesAsync();

    return CreatedAtAction(
        nameof(GetById),
        new { serviceUserId, id = medication.Id },
        medication.ToDto());
}

[HttpPut("{id:guid}")]
public async Task<IActionResult> Update(
    Guid serviceUserId,
    Guid id,
    EditMedicationDto dto)
{
    var medication = await Context.Medications
        .FirstOrDefaultAsync(x =>
            x.ServiceUserId == serviceUserId &&
            x.Id == id);

    if (medication is null)
        return NotFound();

    var validationErrors = await _validator.ValidateUpdateAsync(
        serviceUserId,
        id,
        dto);

    if (validationErrors.Count > 0)
        return BadRequest(validationErrors);

    medication.UpdateFromDto(
        dto,
        GetCurrentUserName());

    await Context.SaveChangesAsync();

    return NoContent();
}

}
