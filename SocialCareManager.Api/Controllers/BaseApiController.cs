using Microsoft.AspNetCore.Mvc;
using SocialCareManager.Infrastructure.Data;

namespace SocialCareManager.Api.Controllers;

public abstract class BaseApiController : ControllerBase
{
    protected readonly ApplicationDbContext Context;

    protected BaseApiController(ApplicationDbContext context)
    {
        Context = context;
    }

    protected string GetCurrentUserName()
    {
        var email = User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(email))
            return "Unknown User";

        var user = Context.Users
            .FirstOrDefault(x => x.Email == email);

        if (user is null)
            return email;

        var fullName = $"{user.FirstName} {user.LastName}".Trim();

        return string.IsNullOrWhiteSpace(fullName)
            ? email
            : fullName;
    }
}