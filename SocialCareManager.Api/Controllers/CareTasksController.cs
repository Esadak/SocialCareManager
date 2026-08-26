using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialCareManager.Api.Dtos.CareTasks;
using SocialCareManager.Domain.Entities;
using SocialCareManager.Domain.Enums;
using SocialCareManager.Infrastructure.Data;

namespace SocialCareManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/serviceusers/{serviceUserId:guid}/caretasks")]
public class CareTasksController : BaseApiController
{
    public CareTasksController(ApplicationDbContext context)
        : base(context)
    {
    }

    // ============================================================
    // GET ALL
    // ============================================================

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CareTaskDto>>> GetAll(Guid serviceUserId)
    {
        var tasks = await Context.CareTasks
            .Where(x => x.ServiceUserId == serviceUserId)
            .OrderBy(x => x.DueAt)
            .ToListAsync();

        var result = tasks.Select(MapToDto);

        return Ok(result);
    }

    // ============================================================
    // GET BY ID
    // ============================================================

    [HttpGet("{taskId:guid}")]
    public async Task<ActionResult<CareTaskDto>> GetById(
        Guid serviceUserId,
        Guid taskId)
    {
        var task = await Context.CareTasks
            .FirstOrDefaultAsync(x =>
                x.Id == taskId &&
                x.ServiceUserId == serviceUserId);

        if (task is null)
            return NotFound();

        return Ok(MapToDto(task));
    }

    // ============================================================
    // CREATE
    // ============================================================

    [HttpPost]
    public async Task<ActionResult<CareTaskDto>> Create(
        Guid serviceUserId,
        CreateCareTaskDto dto)
    {
        var serviceUserExists = await Context.ServiceUsers
            .AnyAsync(x => x.Id == serviceUserId);

        if (!serviceUserExists)
            return NotFound("Service User not found.");

        var task = new CareTask
        {
            Id = Guid.NewGuid(),

            ServiceUserId = serviceUserId,

            Title = dto.Title,

            Description = dto.Description,

            DueAt = DateTime.SpecifyKind(dto.DueAt, DateTimeKind.Utc),

            Priority = dto.Priority,

            AssignedTo = dto.AssignedTo,

            Status = CareTaskStatus.Pending,

            Recurrence = dto.Recurrence,

            RecurrenceInterval = dto.RecurrenceInterval,

            RecurrenceEndDate = dto.RecurrenceEndDate,

            CreatedBy = GetCurrentUserName()
        };

        Context.CareTasks.Add(task);

        await Context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                serviceUserId,
                taskId = task.Id
            },
            MapToDto(task));
    }

    // ============================================================
    // UPDATE
    // ============================================================

    [HttpPut("{taskId:guid}")]
    public async Task<IActionResult> Update(
        Guid serviceUserId,
        Guid taskId,
        UpdateCareTaskDto dto)
    {
        var task = await Context.CareTasks
            .FirstOrDefaultAsync(x =>
                x.Id == taskId &&
                x.ServiceUserId == serviceUserId);

        if (task is null)
            return NotFound();

        task.Title = dto.Title;

        task.Description = dto.Description;

        task.DueAt = DateTime.SpecifyKind(dto.DueAt, DateTimeKind.Utc);

        task.Priority = dto.Priority;

        task.AssignedTo = dto.AssignedTo;

        task.Recurrence = dto.Recurrence;

        task.RecurrenceInterval = dto.RecurrenceInterval;

        task.RecurrenceEndDate = dto.RecurrenceEndDate;

        task.UpdatedAt = DateTime.UtcNow;

        task.UpdatedBy = GetCurrentUserName();

        await Context.SaveChangesAsync();

        return NoContent();
    }


// ============================================================
// START TASK
// ============================================================

[HttpPost("{taskId:guid}/start")]
public async Task<IActionResult> Start(
    Guid serviceUserId,
    Guid taskId)
{
    var task = await Context.CareTasks
        .FirstOrDefaultAsync(x =>
            x.Id == taskId &&
            x.ServiceUserId == serviceUserId);

    if (task is null)
        return NotFound();

    if (task.Status != CareTaskStatus.Pending)
        return BadRequest("Task has already been started.");

    task.Status = CareTaskStatus.InProgress;

    task.StartedAt = DateTime.UtcNow;

    task.StartedBy = GetCurrentUserName();

    task.UpdatedAt = DateTime.UtcNow;

    task.UpdatedBy = GetCurrentUserName();

    await Context.SaveChangesAsync();

    return NoContent();
}

// ============================================================
// COMPLETE TASK
// ============================================================

[HttpPost("{taskId:guid}/complete")]
public async Task<IActionResult> Complete(
    Guid serviceUserId,
    Guid taskId,
    CompleteCareTaskDto dto)
{
    var task = await Context.CareTasks
        .FirstOrDefaultAsync(x =>
            x.Id == taskId &&
            x.ServiceUserId == serviceUserId);

    if (task is null)
        return NotFound();

    if (task.Status == CareTaskStatus.Completed)
        return BadRequest("Task already completed.");

    task.Status = CareTaskStatus.Completed;

    task.CompletedAt = DateTime.UtcNow;

    task.CompletedBy = GetCurrentUserName();

    task.UpdatedAt = DateTime.UtcNow;

    task.UpdatedBy = GetCurrentUserName();

    if (!string.IsNullOrWhiteSpace(dto.Note))
    {
        Context.CareTaskFollowUps.Add(new CareTaskFollowUp
        {
            Id = Guid.NewGuid(),

            CareTaskId = task.Id,

            Note = dto.Note,

            FollowedUpAt = DateTime.UtcNow,

            FollowedUpBy = GetCurrentUserName(),

            CreatedBy = GetCurrentUserName()
        });
    }

    await Context.SaveChangesAsync();

    return NoContent();
}

// ============================================================
// CANCEL TASK
// ============================================================

[HttpPost("{taskId:guid}/cancel")]
public async Task<IActionResult> Cancel(
    Guid serviceUserId,
    Guid taskId,
    CancelCareTaskDto dto)
{
    var task = await Context.CareTasks
        .FirstOrDefaultAsync(x =>
            x.Id == taskId &&
            x.ServiceUserId == serviceUserId);

    if (task is null)
        return NotFound();

    if (task.Status == CareTaskStatus.Completed)
        return BadRequest("Completed task cannot be cancelled.");

    task.Status = CareTaskStatus.Cancelled;

    task.CancelledAt = DateTime.UtcNow;

    task.CancelledBy = GetCurrentUserName();

    task.CancellationReason = dto.Reason;

    task.UpdatedAt = DateTime.UtcNow;

    task.UpdatedBy = GetCurrentUserName();

    await Context.SaveChangesAsync();

    return NoContent();
}

    // ============================================================
    // DTO MAPPING
    // ============================================================

    private static CareTaskDto MapToDto(CareTask task)
    {
        return new CareTaskDto
        {
            Id = task.Id,

            ServiceUserId = task.ServiceUserId,

            Title = task.Title,

            Description = task.Description,

            Status = task.Status,

            Priority = task.Priority,

            DueAt = task.DueAt,

            AssignedTo = task.AssignedTo,

            StartedAt = task.StartedAt,

            CompletedAt = task.CompletedAt,

            CancelledAt = task.CancelledAt,

            Recurrence = task.Recurrence,

            RecurrenceInterval = task.RecurrenceInterval,

            RecurrenceEndDate = task.RecurrenceEndDate,

            CreatedAt = task.CreatedAt,

            UpdatedAt = task.UpdatedAt,

            CreatedBy = task.CreatedBy,

            UpdatedBy = task.UpdatedBy
        };
    }
}