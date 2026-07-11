using SocialCareManager.Api.Dtos.Incident;
using SocialCareManager.Domain.Entities;
using SocialCareManager.Domain.Enums;

namespace SocialCareManager.Api.Validation;

public class IncidentValidator
{
    public List<string> ValidateCreate(CreateIncidentDto dto)
    {
        var errors = new List<string>();

        ValidateCommon(
            dto.Title,
            dto.Category,
            dto.Severity,
            dto.OccurredAt,
            dto.Description,
            dto.ImmediateActions,
            dto.PeopleInvolved,
            errors);

        return errors;
    }

    public List<string> ValidateEdit(
        Incident incident,
        EditIncidentDto dto)
    {
        var errors = new List<string>();

        if (incident.Status == IncidentStatus.Closed)
        {
            errors.Add("A closed incident cannot be edited.");
            return errors;
        }

        ValidateCommon(
            dto.Title,
            dto.Category,
            dto.Severity,
            dto.OccurredAt,
            dto.Description,
            dto.ImmediateActions,
            dto.PeopleInvolved,
            errors);

        return errors;
    }

    public List<string> ValidateStatusChange(
        Incident incident,
        ChangeIncidentStatusDto dto)
    {
        var errors = new List<string>();

        if (incident.Status == IncidentStatus.Closed)
        {
            errors.Add("A closed incident cannot change status.");
            return errors;
        }

        if (dto.Status == IncidentStatus.Closed)
        {
            errors.Add(
                "Use the close incident action when closing an incident.");
        }

        if (dto.Status == incident.Status)
        {
            errors.Add("The incident already has this status.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Note) &&
            dto.Note.Length > 3000)
        {
            errors.Add("The status note cannot exceed 3000 characters.");
        }

        return errors;
    }

    public List<string> ValidateFollowUp(
        Incident incident,
        CreateIncidentFollowUpDto dto)
    {
        var errors = new List<string>();

        if (incident.Status == IncidentStatus.Closed)
        {
            errors.Add("Follow-up cannot be added to a closed incident.");
        }

        if (string.IsNullOrWhiteSpace(dto.Note))
        {
            errors.Add("Enter follow-up information.");
        }
        else if (dto.Note.Length > 3000)
        {
            errors.Add("Follow-up information cannot exceed 3000 characters.");
        }

        return errors;
    }

    public List<string> ValidateClose(
        Incident incident,
        CloseIncidentDto dto)
    {
        var errors = new List<string>();

        if (incident.Status == IncidentStatus.Closed)
        {
            errors.Add("The incident is already closed.");
        }

        if (string.IsNullOrWhiteSpace(dto.Outcome))
        {
            errors.Add("Enter the outcome before closing the incident.");
        }
        else if (dto.Outcome.Length > 3000)
        {
            errors.Add("The outcome cannot exceed 3000 characters.");
        }

        return errors;
    }

    private static void ValidateCommon(
        string title,
        IncidentCategory category,
        IncidentSeverity severity,
        DateTime occurredAt,
        string description,
        string? immediateActions,
        string? peopleInvolved,
        List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            errors.Add("Enter a short title for the incident.");
        }
        else if (title.Length > 200)
        {
            errors.Add("The title cannot exceed 200 characters.");
        }

        if (!Enum.IsDefined(category))
        {
            errors.Add("Select an incident category.");
        }

        if (!Enum.IsDefined(severity))
        {
            errors.Add("Select how serious the incident was.");
        }

        if (occurredAt == default)
        {
            errors.Add("Enter when the incident happened.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            errors.Add("Describe what happened.");
        }
        else if (description.Length > 4000)
        {
            errors.Add("The description cannot exceed 4000 characters.");
        }

        if (!string.IsNullOrWhiteSpace(immediateActions) &&
            immediateActions.Length > 3000)
        {
            errors.Add(
                "Immediate actions cannot exceed 3000 characters.");
        }

        if (!string.IsNullOrWhiteSpace(peopleInvolved) &&
            peopleInvolved.Length > 1000)
        {
            errors.Add(
                "People involved cannot exceed 1000 characters.");
        }
    }
}