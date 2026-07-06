using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialCareManager.Domain.Entities;
using SocialCareManager.Infrastructure.Identity;

namespace SocialCareManager.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { 
    }

    public DbSet<ServiceUser> ServiceUsers => Set<ServiceUser>();

    public DbSet<DailyNote> DailyNotes => Set<DailyNote>();
    public DbSet<NextOfKin> NextOfKin => Set<NextOfKin>();

    public DbSet<CarePlan> CarePlans => Set<CarePlan>();
    public DbSet<Medication> Medications => Set<Medication>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<ServiceUser>()
        .HasQueryFilter(x => !x.IsDeleted);

    modelBuilder.Entity<DailyNote>()
        .HasQueryFilter(x => !x.IsDeleted);

    modelBuilder.Entity<NextOfKin>()
        .HasQueryFilter(x => !x.IsDeleted);

    modelBuilder.Entity<CarePlan>()
        .HasQueryFilter(x => !x.IsDeleted);

    modelBuilder.Entity<Medication>()
        .HasQueryFilter(x => !x.IsDeleted);
}
}