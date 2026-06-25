using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialCareManager.Domain.Entities;
using SocialCareManager.Infrastructure.Data;

namespace SocialCareManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/serviceusers/{serviceUserId:guid}/nextofkin")]
public class NextOfKinController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NextOfKinController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NextOfKin>>> GetAll(Guid serviceUserId)
    {
        var contacts = await _context.NextOfKin
            .Where(x => x.ServiceUserId == serviceUserId)
            .ToListAsync();

        return Ok(contacts);
    }

    [HttpPost]
    public async Task<ActionResult<NextOfKin>> Create(
        Guid serviceUserId,
        NextOfKin contact)
    {
        var serviceUserExists = await _context.ServiceUsers
            .AnyAsync(x => x.Id == serviceUserId);

        if (!serviceUserExists)
            return NotFound("Service user not found.");

        contact.Id = Guid.NewGuid();
        contact.ServiceUserId = serviceUserId;

        _context.NextOfKin.Add(contact);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetAll),
            new { serviceUserId },
            contact);
    }

    [HttpPut("{contactId:guid}")]
    public async Task<IActionResult> Update(
        Guid serviceUserId,
        Guid contactId,
        NextOfKin updatedContact)
    {
        var contact = await _context.NextOfKin
            .FirstOrDefaultAsync(x =>
                x.Id == contactId &&
                x.ServiceUserId == serviceUserId);

        if (contact is null)
            return NotFound();

        contact.FullName = updatedContact.FullName;
        contact.Relationship = updatedContact.Relationship;
        contact.PhoneNumber = updatedContact.PhoneNumber;
        contact.Email = updatedContact.Email;
        contact.Address = updatedContact.Address;
        contact.IsEmergencyContact = updatedContact.IsEmergencyContact;
        contact.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{contactId:guid}")]
    public async Task<IActionResult> Delete(
        Guid serviceUserId,
        Guid contactId)
    {
        var contact = await _context.NextOfKin
            .FirstOrDefaultAsync(x =>
                x.Id == contactId &&
                x.ServiceUserId == serviceUserId);

        if (contact is null)
            return NotFound();

        _context.NextOfKin.Remove(contact);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}