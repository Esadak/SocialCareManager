using SocialCareManager.Domain.Common;

namespace SocialCareManager.Domain.Entities;

public class ServiceUser  : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public ICollection<DailyNote> DailyNotes { get; set; }
        = new List<DailyNote>();
    public ICollection<NextOfKin> NextOfKin { get; set; }
    = new List<NextOfKin>();

    public ICollection<CarePlan> CarePlans { get; set; } 
    = new List<CarePlan>();
    public ICollection<Medication> Medications { get; set; } 
    = new List<Medication>();

    public ICollection<MedicationAdministration> MedicationAdministrations { get; set; }
    = new List<MedicationAdministration>();

    public ICollection<Incident> Incidents { get; set; }
    = new List<Incident>();

    public ICollection<CalendarEvent> CalendarEvents { get; set; }
    = new List<CalendarEvent>();
}