using Microsoft.EntityFrameworkCore;
using Registration.Data;
using Registration.Infrastructure;
using Registration.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole(options =>
{
    options.IncludeScopes = true;
    options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ ";
    options.UseUtcTimestamp = true;
});

builder.Services.AddControllers();
builder.Services.AddDbContext<RegistrationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();
var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();

try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<RegistrationDbContext>();
    dbContext.Database.Migrate();
    startupLogger.LogInformation("Database migrations applied successfully");
}
catch (Exception exception)
{
    startupLogger.LogCritical(exception, "Failed to apply database migrations; application startup aborted");
    throw;
}

app.UseMiddleware<RequestLoggingMiddleware>();
app.MapControllers();
app.Run();
