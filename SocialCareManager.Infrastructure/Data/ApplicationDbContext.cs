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

    public DbSet<MedicationAdministration> MedicationAdministrations =>
    Set<MedicationAdministration>();

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


modelBuilder.Entity<MedicationAdministration>(entity =>
{
    entity.ToTable("MedicationAdministrations");

    entity.HasKey(x => x.Id);

    entity.Property(x => x.Status)
        .HasConversion<string>()
        .HasMaxLength(30);

    entity.Property(x => x.Reason)
        .HasMaxLength(500);

    entity.Property(x => x.Notes)
        .HasMaxLength(2000);

    entity.Property(x => x.AdministeredBy)
        .HasMaxLength(256);

    entity.HasOne(x => x.Medication)
        .WithMany(x => x.Administrations)
        .HasForeignKey(x => x.MedicationId)
        .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(x => x.ServiceUser)
        .WithMany(x => x.MedicationAdministrations)
        .HasForeignKey(x => x.ServiceUserId)
        .OnDelete(DeleteBehavior.Restrict);

    entity.HasIndex(x => x.ServiceUserId);

    entity.HasIndex(x => x.MedicationId);

    entity.HasIndex(x => new
    {
        x.ServiceUserId,
        x.ScheduledAt
    });

    entity.HasIndex(x => new
    {
        x.MedicationId,
        x.ScheduledAt
    });
});

modelBuilder.Entity<MedicationAdministration>()
    .HasQueryFilter(x =>
        !x.IsDeleted &&
        !x.Medication.IsDeleted);
}
}