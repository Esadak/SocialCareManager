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

    public DbSet<ServiceUser> ServiceUsers =>
        Set<ServiceUser>();

    public DbSet<DailyNote> DailyNotes =>
        Set<DailyNote>();

    public DbSet<NextOfKin> NextOfKin =>
        Set<NextOfKin>();

    public DbSet<CarePlan> CarePlans =>
        Set<CarePlan>();

    public DbSet<Medication> Medications =>
        Set<Medication>();

    public DbSet<MedicationAdministration> MedicationAdministrations =>
        Set<MedicationAdministration>();

    public DbSet<Incident> Incidents =>
        Set<Incident>();

    public DbSet<IncidentFollowUp> IncidentFollowUps =>
        Set<IncidentFollowUp>();

    public DbSet<CalendarEvent> CalendarEvents =>
    Set<CalendarEvent>();

    public DbSet<CareTask> CareTasks =>
    Set<CareTask>();

public DbSet<CareTaskFollowUp> CareTaskFollowUps =>
    Set<CareTaskFollowUp>();



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Existing soft-delete filters
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

        modelBuilder.Entity<CarePlan>(entity =>
{
    entity.Property(x => x.ArchivedBy)
        .HasMaxLength(256);

    entity.HasIndex(x => new
    {
        x.ServiceUserId,
        x.IsActive
    });

    entity.HasIndex(x => new
    {
        x.ServiceUserId,
        x.VersionNumber
    });

    entity.HasOne<CarePlan>()
        .WithMany()
        .HasForeignKey(x => x.PreviousVersionId)
        .OnDelete(DeleteBehavior.Restrict);
});

        // Medication Administration
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

        // Incident
        modelBuilder.Entity<Incident>(entity =>
        {
            entity.ToTable("Incidents");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(x => x.Category)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.Severity)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30);

            entity.Property(x => x.Description)
                .HasMaxLength(4000)
                .IsRequired();

            entity.Property(x => x.ImmediateActions)
                .HasMaxLength(3000);

            entity.Property(x => x.PeopleInvolved)
                .HasMaxLength(1000);

            entity.Property(x => x.ManagerNotifiedBy)
                .HasMaxLength(256);

            entity.Property(x => x.Outcome)
                .HasMaxLength(3000);

            entity.Property(x => x.ClosedBy)
                .HasMaxLength(256);

            entity.HasOne(x => x.ServiceUser)
                .WithMany(x => x.Incidents)
                .HasForeignKey(x => x.ServiceUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.FollowUps)
                .WithOne(x => x.Incident)
                .HasForeignKey(x => x.IncidentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.ServiceUserId);

            entity.HasIndex(x => x.Status);

            entity.HasIndex(x => x.Severity);

            entity.HasIndex(x => new
            {
                x.ServiceUserId,
                x.OccurredAt
            });
        });

        // Incident Follow-up
        modelBuilder.Entity<IncidentFollowUp>(entity =>
        {
            entity.ToTable("IncidentFollowUps");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Note)
                .HasMaxLength(3000)
                .IsRequired();

            entity.Property(x => x.FollowedUpBy)
                .HasMaxLength(256);

            entity.HasIndex(x => x.IncidentId);

            entity.HasIndex(x => x.FollowedUpAt);
        });

        modelBuilder.Entity<CalendarEvent>(entity =>
{
    entity.ToTable("CalendarEvents");

    entity.HasKey(x => x.Id);

    entity.Property(x => x.Title)
        .HasMaxLength(200)
        .IsRequired();

    entity.Property(x => x.EventType)
        .HasConversion<string>()
        .HasMaxLength(40);

    entity.Property(x => x.Status)
        .HasConversion<string>()
        .HasMaxLength(20);

    entity.Property(x => x.Location)
        .HasMaxLength(300);

    entity.Property(x => x.Description)
        .HasMaxLength(3000);

    entity.Property(x => x.AssignedTo)
        .HasMaxLength(256);

    entity.Property(x => x.CompletedBy)
        .HasMaxLength(256);

    entity.Property(x => x.CancelledBy)
        .HasMaxLength(256);

    entity.Property(x => x.CancellationReason)
        .HasMaxLength(1000);

    entity.HasOne(x => x.ServiceUser)
        .WithMany(x => x.CalendarEvents)
        .HasForeignKey(x => x.ServiceUserId)
        .OnDelete(DeleteBehavior.Restrict);

    entity.HasIndex(x => x.ServiceUserId);

    entity.HasIndex(x => x.Status);

    entity.HasIndex(x => x.EventType);

    entity.HasIndex(x => x.StartAt);

    entity.HasIndex(x => new
    {
        x.ServiceUserId,
        x.StartAt
    });

    entity.HasIndex(x => new
    {
        x.ServiceUserId,
        x.Status
    });
});

        // Matching query filters for required relationships
        modelBuilder.Entity<MedicationAdministration>()
            .HasQueryFilter(x =>
                !x.IsDeleted &&
                !x.Medication.IsDeleted &&
                !x.ServiceUser.IsDeleted);

        modelBuilder.Entity<Incident>()
            .HasQueryFilter(x =>
                !x.IsDeleted &&
                !x.ServiceUser.IsDeleted);

        modelBuilder.Entity<IncidentFollowUp>()
            .HasQueryFilter(x =>
                !x.IsDeleted &&
                !x.Incident.IsDeleted &&
                !x.Incident.ServiceUser.IsDeleted);

        modelBuilder.Entity<CalendarEvent>()
              .HasQueryFilter(x =>
              !x.IsDeleted &&
              !x.ServiceUser.IsDeleted);



        modelBuilder.Entity<CareTask>(entity =>
{
    entity.ToTable("CareTasks");

    entity.HasKey(x => x.Id);

    entity.Property(x => x.Title)
        .HasMaxLength(200)
        .IsRequired();

    entity.Property(x => x.Description)
        .HasMaxLength(4000);

    entity.Property(x => x.Status)
        .HasConversion<string>()
        .HasMaxLength(30);

    entity.Property(x => x.Priority)
        .HasConversion<string>()
        .HasMaxLength(20);

    entity.Property(x => x.Recurrence)
        .HasConversion<string>()
        .HasMaxLength(20);

    entity.Property(x => x.AssignedTo)
        .HasMaxLength(256);

    entity.Property(x => x.StartedBy)
        .HasMaxLength(256);

    entity.Property(x => x.CompletedBy)
        .HasMaxLength(256);

    entity.Property(x => x.CancelledBy)
        .HasMaxLength(256);

    entity.Property(x => x.CancellationReason)
        .HasMaxLength(1000);

    entity.HasOne(x => x.ServiceUser)
        .WithMany(x => x.CareTasks)
        .HasForeignKey(x => x.ServiceUserId)
        .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(x => x.ParentTask)
        .WithMany(x => x.GeneratedTasks)
        .HasForeignKey(x => x.ParentTaskId)
        .OnDelete(DeleteBehavior.Restrict);

    entity.HasMany(x => x.FollowUps)
        .WithOne(x => x.CareTask)
        .HasForeignKey(x => x.CareTaskId)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<CareTaskFollowUp>(entity =>
{
    entity.ToTable("CareTaskFollowUps");

    entity.HasKey(x => x.Id);

    entity.Property(x => x.Note)
        .HasMaxLength(3000)
        .IsRequired();

    entity.Property(x => x.FollowedUpBy)
        .HasMaxLength(256);

    entity.HasIndex(x => x.CareTaskId);

    entity.HasIndex(x => x.FollowedUpAt);
});

    entity.HasIndex(x => x.ServiceUserId);

    entity.HasIndex(x => x.Status);

    entity.HasIndex(x => x.Priority);

    entity.HasIndex(x => x.DueAt);

    entity.HasIndex(x => x.AssignedTo);

    entity.HasIndex(x => x.ParentTaskId);

    modelBuilder.Entity<CareTask>()
    .HasQueryFilter(x =>
        !x.IsDeleted &&
        !x.ServiceUser.IsDeleted);

modelBuilder.Entity<CareTaskFollowUp>()
    .HasQueryFilter(x =>
        !x.IsDeleted &&
        !x.CareTask.IsDeleted &&
        !x.CareTask.ServiceUser.IsDeleted);

    entity.HasIndex(x => new
    {
        x.ServiceUserId,
        x.Status
    });

    entity.HasIndex(x => new
    {
        x.ServiceUserId,
        x.DueAt
    });
});
    }
}