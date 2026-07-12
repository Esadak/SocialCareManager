namespace SocialCareManager.Web.Dtos.Dashboard;

public class DashboardInsightsDto
{
    public int TotalServiceUsers { get; set; }

    public int ActiveCarePlans { get; set; }

    public int CarePlansOverdue { get; set; }

    public int CarePlansDueSoon { get; set; }

    public int OpenIncidents { get; set; }

    public int HighSeverityIncidents { get; set; }

    public int MedicationWaiting { get; set; }

    public int MedicationMissed { get; set; }

    public int CalendarToday { get; set; }

    public int CalendarOverdue { get; set; }

    public int MissingDailyNotes { get; set; }

    public int MissingNextOfKin { get; set; }
}