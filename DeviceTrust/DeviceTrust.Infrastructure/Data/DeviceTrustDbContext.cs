using DeviceTrust.Domain.Entities;
using DeviceTrust.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace DeviceTrust.Infrastructure.Data;

public class DeviceTrustDbContext : IdentityDbContext<ApplicationUser>
{
    public DeviceTrustDbContext(DbContextOptions<DeviceTrustDbContext> options)
        : base(options) { }

    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Ownership> Ownerships => Set<Ownership>();
    public DbSet<OwnershipTransfer> OwnershipTransfers => Set<OwnershipTransfer>();
    public DbSet<RepairCenter> RepairCenters => Set<RepairCenter>();
    public DbSet<TechnicianProfile> TechnicianProfiles => Set<TechnicianProfile>();
    public DbSet<RepairRecord> RepairRecords => Set<RepairRecord>();
    public DbSet<RepairPart> RepairParts => Set<RepairPart>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ---- Device ----
        builder.Entity<Device>()
            .HasIndex(d => d.PublicPassportId)
            .IsUnique();

        // ---- Ownership ----
        builder.Entity<Ownership>()
            .HasIndex(o => o.DeviceId)
            .IsUnique()
            .HasFilter("[EndDate] IS NULL");

        builder.Entity<Ownership>()
            .HasOne(o => o.Device)
            .WithMany(d => d.Ownerships)
            .HasForeignKey(o => o.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---- OwnershipTransfer ----
        // Only one Pending transfer allowed per device at a time.
        builder.Entity<OwnershipTransfer>()
            .HasIndex(t => t.DeviceId)
            .IsUnique()
            .HasFilter("[Status] = 1"); // 1 = Pending (matches enum's underlying int value)

        builder.Entity<OwnershipTransfer>()
            .HasOne(t => t.Device)
            .WithMany(d => d.Transfers)
            .HasForeignKey(t => t.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---- TechnicianProfile ----
        // One-to-one with ApplicationUser: a user has at most one TechnicianProfile.
        builder.Entity<TechnicianProfile>()
            .HasIndex(t => t.UserId)
            .IsUnique();

        builder.Entity<TechnicianProfile>()
            .HasOne(t => t.RepairCenter)
            .WithMany(rc => rc.Technicians)
            .HasForeignKey(t => t.RepairCenterId)
            .OnDelete(DeleteBehavior.Restrict); // unlinking sets FK to null in code, never cascade-deletes

        // ---- RepairRecord ----
        builder.Entity<RepairRecord>()
            .HasOne(r => r.Device)
            .WithMany(d => d.RepairRecords)
            .HasForeignKey(r => r.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RepairRecord>()
            .HasOne(r => r.TechnicianProfile)
            .WithMany(t => t.RepairRecords)
            .HasForeignKey(r => r.TechnicianProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        // Self-referencing FK for corrections — must be Restrict, or deleting a record
        // could cascade into deleting its corrections (and we never delete Verified records anyway).
        builder.Entity<RepairRecord>()
            .HasOne(r => r.CorrectsRecord)
            .WithMany()
            .HasForeignKey(r => r.CorrectsRecordId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---- RepairPart ----
        builder.Entity<RepairPart>()
            .HasOne(p => p.RepairRecord)
            .WithMany(r => r.Parts)
            .HasForeignKey(p => p.RepairRecordId)
            .OnDelete(DeleteBehavior.Cascade); // parts have no meaning without their parent record

        // ---- Attachment ----
        builder.Entity<Attachment>()
            .HasOne(a => a.RepairRecord)
            .WithMany(r => r.Attachments)
            .HasForeignKey(a => a.RepairRecordId)
            .OnDelete(DeleteBehavior.Cascade); // same reasoning as RepairPart
    }
}