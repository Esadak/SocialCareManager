using SocialCareManager.Api.Dtos.Calendar;
using SocialCareManager.Domain.Entities;
using SocialCareManager.Domain.Enums;

namespace SocialCareManager.Api.Mapping;

public static class CalendarEventMapper
{
    public static CalendarEventDto ToDto(
        this CalendarEvent calendarEvent)
    {
        return new CalendarEventDto
        {
            Id = calendarEvent.Id,
            ServiceUserId = calendarEvent.ServiceUserId,

            Title = calendarEvent.Title,
            EventType = calendarEvent.EventType,
            Status = calendarEvent.Status,

            StartAt = calendarEvent.StartAt,
            EndAt = calendarEvent.EndAt,
            IsAllDay = calendarEvent.IsAllDay,

            Location = calendarEvent.Location,
            Description = calendarEvent.Description,
            AssignedTo = calendarEvent.AssignedTo,

            ReminderMinutesBefore =
                calendarEvent.ReminderMinutesBefore,

            CompletedAt = calendarEvent.CompletedAt,
            CompletedBy = calendarEvent.CompletedBy,

            CancelledAt = calendarEvent.CancelledAt,
            CancelledBy = calendarEvent.CancelledBy,
            CancellationReason = calendarEvent.CancellationReason,

            CreatedAt = calendarEvent.CreatedAt,
            CreatedBy = calendarEvent.CreatedBy,
            UpdatedAt = calendarEvent.UpdatedAt,
            UpdatedBy = calendarEvent.UpdatedBy
        };
    }

    public static CalendarEvent ToEntity(
        this CreateCalendarEventDto dto,
        Guid serviceUserId,
        string? currentUser)
    {
        return new CalendarEvent
        {
            Id = Guid.NewGuid(),
            ServiceUserId = serviceUserId,

            Title = dto.Title.Trim(),
            EventType = dto.EventType,
            Status = CalendarEventStatus.Scheduled,

            StartAt = EnsureUtc(dto.StartAt),
            EndAt = EnsureUtc(dto.EndAt),
            IsAllDay = dto.IsAllDay,

            Location = dto.Location?.Trim(),
            Description = dto.Description?.Trim(),
            AssignedTo = dto.AssignedTo?.Trim(),

            ReminderMinutesBefore =
                dto.ReminderMinutesBefore,

            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUser
        };
    }

    public static void UpdateFromDto(
        this CalendarEvent calendarEvent,
        EditCalendarEventDto dto,
        string? currentUser)
    {
        calendarEvent.Title = dto.Title.Trim();
        calendarEvent.EventType = dto.EventType;

        calendarEvent.StartAt = EnsureUtc(dto.StartAt);
        calendarEvent.EndAt = EnsureUtc(dto.EndAt);
        calendarEvent.IsAllDay = dto.IsAllDay;

        calendarEvent.Location = dto.Location?.Trim();
        calendarEvent.Description = dto.Description?.Trim();
        calendarEvent.AssignedTo = dto.AssignedTo?.Trim();

        calendarEvent.ReminderMinutesBefore =
            dto.ReminderMinutesBefore;

        calendarEvent.UpdatedAt = DateTime.UtcNow;
        calendarEvent.UpdatedBy = currentUser;
    }

    public static void Complete(
        this CalendarEvent calendarEvent,
        string? currentUser)
    {
        calendarEvent.Status =
            CalendarEventStatus.Completed;

        calendarEvent.CompletedAt = DateTime.UtcNow;
        calendarEvent.CompletedBy = currentUser;

        calendarEvent.CancelledAt = null;
        calendarEvent.CancelledBy = null;
        calendarEvent.CancellationReason = null;

        calendarEvent.UpdatedAt = DateTime.UtcNow;
        calendarEvent.UpdatedBy = currentUser;
    }

    public static void Cancel(
        this CalendarEvent calendarEvent,
        string reason,
        string? currentUser)
    {
        calendarEvent.Status =
            CalendarEventStatus.Cancelled;

        calendarEvent.CancelledAt = DateTime.UtcNow;
        calendarEvent.CancelledBy = currentUser;
        calendarEvent.CancellationReason = reason.Trim();

        calendarEvent.CompletedAt = null;
        calendarEvent.CompletedBy = null;

        calendarEvent.UpdatedAt = DateTime.UtcNow;
        calendarEvent.UpdatedBy = currentUser;
    }

    public static void Reopen(
        this CalendarEvent calendarEvent,
        string? currentUser)
    {
        calendarEvent.Status =
            CalendarEventStatus.Scheduled;

        calendarEvent.CompletedAt = null;
        calendarEvent.CompletedBy = null;

        calendarEvent.CancelledAt = null;
        calendarEvent.CancelledBy = null;
        calendarEvent.CancellationReason = null;

        calendarEvent.UpdatedAt = DateTime.UtcNow;
        calendarEvent.UpdatedBy = currentUser;
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}