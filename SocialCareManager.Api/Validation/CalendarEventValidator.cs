using SocialCareManager.Api.Dtos.Calendar;
using SocialCareManager.Domain.Entities;
using SocialCareManager.Domain.Enums;

namespace SocialCareManager.Api.Validation;

public class CalendarEventValidator
{
    public List<string> ValidateCreate(
        CreateCalendarEventDto dto)
    {
        var errors = new List<string>();

        ValidateCommon(
            dto.Title,
            dto.EventType,
            dto.StartAt,
            dto.EndAt,
            dto.IsAllDay,
            dto.Location,
            dto.Description,
            dto.AssignedTo,
            dto.ReminderMinutesBefore,
            errors);

        return errors;
    }

    public List<string> ValidateEdit(
        CalendarEvent calendarEvent,
        EditCalendarEventDto dto)
    {
        var errors = new List<string>();

        if (calendarEvent.Status !=
            CalendarEventStatus.Scheduled)
        {
            errors.Add(
                "Only scheduled calendar events can be edited.");

            return errors;
        }

        ValidateCommon(
            dto.Title,
            dto.EventType,
            dto.StartAt,
            dto.EndAt,
            dto.IsAllDay,
            dto.Location,
            dto.Description,
            dto.AssignedTo,
            dto.ReminderMinutesBefore,
            errors);

        return errors;
    }

    public List<string> ValidateComplete(
        CalendarEvent calendarEvent)
    {
        var errors = new List<string>();

        if (calendarEvent.Status ==
            CalendarEventStatus.Completed)
        {
            errors.Add(
                "The calendar event is already completed.");
        }

        if (calendarEvent.Status ==
            CalendarEventStatus.Cancelled)
        {
            errors.Add(
                "A cancelled calendar event cannot be completed.");
        }

        return errors;
    }

    public List<string> ValidateCancel(
        CalendarEvent calendarEvent,
        CancelCalendarEventDto dto)
    {
        var errors = new List<string>();

        if (calendarEvent.Status ==
            CalendarEventStatus.Cancelled)
        {
            errors.Add(
                "The calendar event is already cancelled.");
        }

        if (calendarEvent.Status ==
            CalendarEventStatus.Completed)
        {
            errors.Add(
                "A completed calendar event cannot be cancelled.");
        }

        if (string.IsNullOrWhiteSpace(dto.Reason))
        {
            errors.Add(
                "Enter why the calendar event was cancelled.");
        }
        else if (dto.Reason.Length > 1000)
        {
            errors.Add(
                "The cancellation reason cannot exceed 1000 characters.");
        }

        return errors;
    }

    public List<string> ValidateReopen(
        CalendarEvent calendarEvent,
        ReopenCalendarEventDto dto)
    {
        var errors = new List<string>();

        if (calendarEvent.Status ==
            CalendarEventStatus.Scheduled)
        {
            errors.Add(
                "The calendar event is already scheduled.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Reason) &&
            dto.Reason.Length > 1000)
        {
            errors.Add(
                "The reason cannot exceed 1000 characters.");
        }

        return errors;
    }

    private static void ValidateCommon(
        string title,
        CalendarEventType eventType,
        DateTime startAt,
        DateTime endAt,
        bool isAllDay,
        string? location,
        string? description,
        string? assignedTo,
        int? reminderMinutesBefore,
        List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            errors.Add("Enter a title.");
        }
        else if (title.Length > 200)
        {
            errors.Add(
                "The title cannot exceed 200 characters.");
        }

        if (!Enum.IsDefined(eventType))
        {
            errors.Add(
                "Select a calendar event type.");
        }

        if (startAt == default)
        {
            errors.Add(
                "Enter when the event starts.");
        }

        if (endAt == default)
        {
            errors.Add(
                "Enter when the event ends.");
        }

        if (startAt != default &&
            endAt != default &&
            endAt <= startAt)
        {
            errors.Add(
                "The end time must be after the start time.");
        }

        if (!string.IsNullOrWhiteSpace(location) &&
            location.Length > 300)
        {
            errors.Add(
                "The location cannot exceed 300 characters.");
        }

        if (!string.IsNullOrWhiteSpace(description) &&
            description.Length > 3000)
        {
            errors.Add(
                "The description cannot exceed 3000 characters.");
        }

        if (!string.IsNullOrWhiteSpace(assignedTo) &&
            assignedTo.Length > 256)
        {
            errors.Add(
                "The assigned person cannot exceed 256 characters.");
        }

        if (reminderMinutesBefore.HasValue &&
            reminderMinutesBefore.Value < 0)
        {
            errors.Add(
                "Reminder time cannot be negative.");
        }

        if (reminderMinutesBefore.HasValue &&
            reminderMinutesBefore.Value > 10080)
        {
            errors.Add(
                "Reminder time cannot exceed seven days.");
        }
    }
}