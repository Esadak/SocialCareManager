using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialCareManager.Domain.Entities;
using SocialCareManager.Infrastructure.Data;

namespace SocialCareManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/serviceusers/{serviceUserId:guid}/dailynotes")]
public class DailyNotesController : BaseApiController
{
    public DailyNotesController(ApplicationDbContext context)
    : base(context)
{
}

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DailyNote>>> GetAll(Guid serviceUserId)
    {
        var notes = await Context.DailyNotes
            .Where(x => x.ServiceUserId == serviceUserId)
            .ToListAsync();

        return Ok(notes);
    }

    [HttpPost]
    public async Task<ActionResult<DailyNote>> Create(
        Guid serviceUserId,
        DailyNote note)
    {
        var serviceUserExists = await Context.ServiceUsers
            .AnyAsync(x => x.Id == serviceUserId);

        if (!serviceUserExists)
            return NotFound("Service user not found.");

        note.Id = Guid.NewGuid();
        note.ServiceUserId = serviceUserId;
        note.CreatedBy = GetCurrentUserName();
        note.UpdatedBy = null;

        Context.DailyNotes.Add(note);

        await Context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetAll),
            new { serviceUserId },
            note);
    }

    [HttpPut("{noteId:guid}")]
    public async Task<IActionResult> Update(
        Guid serviceUserId,
        Guid noteId,
        DailyNote updatedNote)
    {
        var note = await Context.DailyNotes
            .FirstOrDefaultAsync(x =>
                x.Id == noteId &&
                x.ServiceUserId == serviceUserId);

        if (note is null)
            return NotFound();

        note.Title = updatedNote.Title;
        note.Content = updatedNote.Content;
        note.UpdatedAt = DateTime.UtcNow;
        note.UpdatedBy = GetCurrentUserName();

        await Context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{noteId:guid}")]
    public async Task<IActionResult> Delete(
        Guid serviceUserId,
        Guid noteId)
    {
        var note = await Context.DailyNotes
            .FirstOrDefaultAsync(x =>
                x.Id == noteId &&
                x.ServiceUserId == serviceUserId);

        if (note is null)
            return NotFound();

        note.MarkAsDeleted(GetCurrentUserName());

        await Context.SaveChangesAsync();

        return NoContent();
    }

}