using DeviceTrust.Infrastructure.Data;
using DeviceTrust.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DeviceTrust.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DeviceTrustDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DeviceTrustDb")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8; // tune later
})
    .AddEntityFrameworkStores<DeviceTrustDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await DbInitializer.SeedAsync(scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication(); // must come before UseAuthorization
app.UseAuthorization();
app.MapControllers();
app.Run();