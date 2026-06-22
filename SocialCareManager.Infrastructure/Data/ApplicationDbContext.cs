using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialCareManager.Domain.Entities;

namespace SocialCareManager.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ServiceUser> ServiceUsers => Set<ServiceUser>();

    public DbSet<DailyNote> DailyNotes => Set<DailyNote>();
}