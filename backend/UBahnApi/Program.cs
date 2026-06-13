using Microsoft.EntityFrameworkCore;
using UBahnApi.Data;
using UBahnApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("UBahn");
builder.Services.AddDbContext<UBahnContext>(opt =>
    opt.UseMySql(connectionString, new MariaDbServerVersion(new Version(10, 11))));

builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

DbBootstrapper.EnsureDatabaseAndUser(app.Configuration);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UBahnContext>();
    db.Database.Migrate();
    await DataSeeder.SeedAsync(db);
}

app.Run();
