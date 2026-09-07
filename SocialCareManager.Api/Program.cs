using SocialCareManager.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialCareManager.Infrastructure.Data;
using SocialCareManager.Api.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<MedicationValidator>();
builder.Services.AddScoped<MedicationAdministrationValidator>();
builder.Services.AddScoped<IncidentValidator>();
builder.Services.AddScoped<CalendarEventValidator>();
builder.Services.AddScoped<CarePlanValidator>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}



app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (context.Request.Path.Equals("/register", StringComparison.OrdinalIgnoreCase) &&
        HttpMethods.IsPost(context.Request.Method))
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync(
            "Public registration is disabled. Contact an administrator to get an account created.");
        return;
    }

    await next();
});

app.MapIdentityApi<ApplicationUser>();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = ["Admin", "Staff"];

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var adminEmail = "admin@socialcaremanager.local";
    
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser is not null)
{
    adminUser.FirstName = "Admin";
    adminUser.LastName = "User";
    await userManager.UpdateAsync(adminUser);
}

    if (adminUser is not null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    /*staff*/
    var staffEmail = "staff@socialcaremanager.local";

var staffUser = await userManager.FindByEmailAsync(staffEmail);

if (staffUser == null)
{
    staffUser = new ApplicationUser
    {
        UserName = staffEmail,
        Email = staffEmail,
        EmailConfirmed = true,
        FirstName = "Staff",
        LastName = "User"
    };

    await userManager.CreateAsync(staffUser, "Staff123!");
}

if (staffUser is not null)
{
    staffUser.FirstName = "Staff";
    staffUser.LastName = "User";
    await userManager.UpdateAsync(staffUser);
}

if (staffUser is not null &&
    !await userManager.IsInRoleAsync(staffUser, "Staff"))
{
    await userManager.AddToRoleAsync(staffUser, "Staff");
}
}
}

app.Run();