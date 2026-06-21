using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialCareManager.Domain.Entities;
using SocialCareManager.Infrastructure.Data;

namespace SocialCareManager.Api.Controllers;

[ApiController]
[Route("api/serviceusers/{serviceUserId:guid}/dailynotes")]
public class DailyNotesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DailyNotesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DailyNote>>> GetAll(
        Guid serviceUserId)
    {
        var notes = await _context.DailyNotes
            .Where(x => x.ServiceUserId == serviceUserId)
            .ToListAsync();

        return Ok(notes);
    }

    [HttpPost]
    public async Task<ActionResult<DailyNote>> Create(
        Guid serviceUserId,
        DailyNote note)
    {
        var serviceUserExists = await _context.ServiceUsers
            .AnyAsync(x => x.Id == serviceUserId);

        if (!serviceUserExists)
            return NotFound("Service user not found.");

        note.Id = Guid.NewGuid();
        note.ServiceUserId = serviceUserId;

        _context.DailyNotes.Add(note);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetAll),
            new { serviceUserId },
            note);
    }
                                                    /*Delete */
    [HttpDelete("{noteId:guid}")]
public async Task<IActionResult> Delete(
    Guid serviceUserId,
    Guid noteId)
{
    var note = await _context.DailyNotes
        .FirstOrDefaultAsync(x =>
            x.Id == noteId &&
            x.ServiceUserId == serviceUserId);

    if (note is null)
        return NotFound();

    _context.DailyNotes.Remove(note);

    await _context.SaveChangesAsync();

    return NoContent();
}
                                                            /*update */
[HttpPut("{noteId:guid}")]
public async Task<IActionResult> Update(
    Guid serviceUserId,
    Guid noteId,
    DailyNote updatedNote)
{
    var note = await _context.DailyNotes
        .FirstOrDefaultAsync(x =>
            x.Id == noteId &&
            x.ServiceUserId == serviceUserId);

    if (note is null)
        return NotFound();

    note.Title = updatedNote.Title;
    note.Content = updatedNote.Content;
    note.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return NoContent();
}
}