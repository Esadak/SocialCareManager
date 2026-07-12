using SocialCareManager.Api.Dtos;
using SocialCareManager.Domain.Entities;
using SocialCareManager.Api.Validation;

namespace SocialCareManager.Api.Validation;

public class CarePlanValidator
{
    public List<string> ValidateCreate(CreateCarePlanDto dto)
    {
        var errors = new List<string>();

        ValidateCommon(
            dto.Goal,
            dto.Needs,
            dto.SupportPlan,
            dto.RiskAssessment,
            dto.ReviewDate,
            errors);

        return errors;
    }

    public List<string> ValidateUpdate(
        CarePlan carePlan,
        UpdateCarePlanDto dto)
    {
        var errors = new List<string>();

        if (!carePlan.IsActive)
        {
            errors.Add("Only the active care plan can be updated.");
            return errors;
        }

        ValidateCommon(
            dto.Goal,
            dto.Needs,
            dto.SupportPlan,
            dto.RiskAssessment,
            dto.ReviewDate,
            errors);

        return errors;
    }

    public List<string> ValidateCreateNewVersion(
        CarePlan currentPlan,
        CreateCarePlanDto dto)
    {
        var errors = new List<string>();

        if (!currentPlan.IsActive)
        {
            errors.Add("A new version can only be created from the active care plan.");
            return errors;
        }

        ValidateCommon(
            dto.Goal,
            dto.Needs,
            dto.SupportPlan,
            dto.RiskAssessment,
            dto.ReviewDate,
            errors);

        return errors;
    }

    private static void ValidateCommon(
        string goal,
        string needs,
        string supportPlan,
        string riskAssessment,
        DateTime reviewDate,
        List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(goal))
        {
            errors.Add("Enter the main goal.");
        }
        else if (goal.Length > 3000)
        {
            errors.Add("The goal cannot exceed 3000 characters.");
        }

        if (string.IsNullOrWhiteSpace(needs))
        {
            errors.Add("Describe the person's needs.");
        }
        else if (needs.Length > 3000)
        {
            errors.Add("Needs cannot exceed 3000 characters.");
        }

        if (string.IsNullOrWhiteSpace(supportPlan))
        {
            errors.Add("Describe how support should be provided.");
        }
        else if (supportPlan.Length > 5000)
        {
            errors.Add("The support plan cannot exceed 5000 characters.");
        }

        if (!string.IsNullOrWhiteSpace(riskAssessment) &&
            riskAssessment.Length > 3000)
        {
            errors.Add("The risk assessment cannot exceed 3000 characters.");
        }

        if (reviewDate == default)
        {
            errors.Add("Enter a review date.");
        }
    }
}