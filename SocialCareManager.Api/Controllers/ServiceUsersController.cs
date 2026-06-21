using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialCareManager.Domain.Entities;
using SocialCareManager.Infrastructure.Data;

namespace SocialCareManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceUsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ServiceUsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceUser>>> GetAll()
    {
        return await _context.ServiceUsers.ToListAsync();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ServiceUser>> GetById(Guid id)
    {
        var serviceUser = await _context.ServiceUsers.FindAsync(id);

        if (serviceUser == null)
            return NotFound();

        return serviceUser;
    }

    [HttpPost]
public async Task<ActionResult<ServiceUser>> Create(ServiceUser serviceUser)
{
    serviceUser.Id = Guid.NewGuid();
    serviceUser.DateOfBirth = DateTime.SpecifyKind(serviceUser.DateOfBirth, DateTimeKind.Utc);

    _context.ServiceUsers.Add(serviceUser);

    await _context.SaveChangesAsync();

    return CreatedAtAction(
        nameof(GetById),
        new { id = serviceUser.Id },
        serviceUser);
}
}