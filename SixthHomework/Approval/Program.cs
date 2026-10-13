using Approval.Data;
using Approval.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<ApprovalDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres"),
        postgres => postgres.MigrationsHistoryTable("__EFMigrationsHistory_Approval")));
builder.Services.AddScoped<IApprovalService, ApprovalService>();

var app = builder.Build();
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApprovalDbContext>();
    await db.Database.MigrateAsync();
}

app.MapControllers();
app.Run();
