using DeviceTrust.Domain.Entities;
using DeviceTrust.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace DeviceTrust.Infrastructure.Data;

public class DeviceTrustDbContext : IdentityDbContext<ApplicationUser>
{
    public DeviceTrustDbContext(DbContextOptions<DeviceTrustDbContext> options)
        : base(options) { }

    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Ownership> Ownerships => Set<Ownership>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // MUST run first — sets up Identity's own tables

        builder.Entity<Device>()
            .HasIndex(d => d.PublicPassportId)
            .IsUnique();

        builder.Entity<Ownership>()
            .HasIndex(o => o.DeviceId)
            .IsUnique()
            .HasFilter("[EndDate] IS NULL");

        builder.Entity<Ownership>()
            .HasOne(o => o.Device)
            .WithMany(d => d.Ownerships)
            .HasForeignKey(o => o.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}