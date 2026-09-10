using DeviceTrust.Infrastructure.Auth;
using DeviceTrust.Infrastructure.Data;
using DeviceTrust.Infrastructure.Devices;
using DeviceTrust.Infrastructure.Identity;
using DeviceTrust.Infrastructure.RepairCenters;
using DeviceTrust.Infrastructure.Repairs;
using DeviceTrust.Infrastructure.Transfers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://YOUR-MONSTERASP-DOMAIN.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<DeviceTrustDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DeviceTrustDb")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
})
    .AddEntityFrameworkStores<DeviceTrustDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<TransferService>();

builder.Services.AddScoped<RepairCenterService>();

builder.Services.AddScoped<RepairService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false; // <-- add this line: keep "sub" as "sub", don't remap to legacy claim URIs
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });


builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<PassportIdGenerator>();
builder.Services.AddScoped<DeviceService>();

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

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapFallbackToFile("index.html");

app.Run();