using DeviceTrust.Domain.Entities;
using DeviceTrust.Domain.Enums;
using DeviceTrust.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeviceTrust.Infrastructure.Repairs;

public class RepairService
{
    private readonly DeviceTrustDbContext _context;

    public RepairService(DeviceTrustDbContext context)
    {
        _context = context;
    }

    private async Task<(bool trusted, string? error, TechnicianProfile? profile)> CheckTechnicianTrustAsync(string userId)
    {
        var profile = await _context.TechnicianProfiles
            .Include(t => t.RepairCenter)
            .FirstOrDefaultAsync(t => t.UserId == userId);

        if (profile is null)
            return (false, "No technician profile found for this user.", null);

        if (profile.RepairCenterId is null || profile.RepairCenter is null)
            return (false, "Technician is not linked to a repair center.", null);

        if (!profile.IsApproved)
            return (false, "Technician is not approved.", null);

        if (!profile.RepairCenter.IsApproved)
            return (false, "Technician's repair center is not approved.", null);

        return (true, null, profile);
    }

    public async Task<(bool success, string? error, RepairRecord? record)> CreateDraftAsync(
        int deviceId, string technicianUserId, CreateRepairInput input)
    {
        var (trusted, error, profile) = await CheckTechnicianTrustAsync(technicianUserId);
        if (!trusted) return (false, error, null);

        var device = await _context.Devices.FindAsync(deviceId);
        if (device is null) return (false, "Device not found.", null);

        if (input.CorrectsRecordId.HasValue)
        {
            var original = await _context.RepairRecords.FindAsync(input.CorrectsRecordId.Value);
            if (original is null) return (false, "The record being corrected was not found.", null);
            if (original.Status != RepairStatus.Verified)
                return (false, "Can only correct a Verified record. Edit the Draft directly instead.", null);
        }

        var record = new RepairRecord
        {
            DeviceId = deviceId,
            TechnicianProfileId = profile!.Id,
            RecordType = RecordType.Repair,
            Status = RepairStatus.Draft,
            ProblemDescription = input.ProblemDescription,
            Diagnosis = input.Diagnosis,
            ActionTaken = input.ActionTaken,
            RepairDate = input.RepairDate,
            WarrantyUntil = input.WarrantyUntil,
            CreatedAt = DateTime.UtcNow,
            CorrectsRecordId = input.CorrectsRecordId
        };

        _context.RepairRecords.Add(record);
        await _context.SaveChangesAsync();

        return (true, null, record);
    }

    public async Task<RepairRecord?> GetOwnedDraftAsync(int repairId, string technicianUserId)
    {
        return await _context.RepairRecords
            .Include(r => r.TechnicianProfile)
            .Include(r => r.Parts)
            .FirstOrDefaultAsync(r => r.Id == repairId
                && r.TechnicianProfile.UserId == technicianUserId
                && r.Status == RepairStatus.Draft);
    }

    public async Task<(bool success, string? error)> UpdateDraftAsync(
        int repairId, string technicianUserId, UpdateRepairInput input)
    {
        var record = await GetOwnedDraftAsync(repairId, technicianUserId);
        if (record is null) return (false, "Draft repair record not found, or it is no longer editable.");

        record.ProblemDescription = input.ProblemDescription;
        record.Diagnosis = input.Diagnosis;
        record.ActionTaken = input.ActionTaken;
        record.RepairDate = input.RepairDate;
        record.WarrantyUntil = input.WarrantyUntil;

        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool success, string? error)> AddPartAsync(int repairId, string technicianUserId, RepairPart part)
    {
        var record = await GetOwnedDraftAsync(repairId, technicianUserId);
        if (record is null) return (false, "Draft repair record not found, or it is no longer editable.");

        part.RepairRecordId = repairId;
        _context.RepairParts.Add(part);
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool success, string? error)> SubmitAsync(int repairId, string technicianUserId)
    {
        var record = await GetOwnedDraftAsync(repairId, technicianUserId);
        if (record is null) return (false, "Draft repair record not found, or already submitted.");

        var (trusted, error, _) = await CheckTechnicianTrustAsync(technicianUserId);
        if (!trusted) return (false, $"Cannot submit: {error}");

        record.Status = RepairStatus.Verified;
        record.VerifiedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _context.AuditLogs.Add(new AuditLog
        {
            EntityType = nameof(RepairRecord),
            EntityId = record.Id,
            Action = "Verified",
            PerformedByUserId = technicianUserId,
            Timestamp = DateTime.UtcNow,
            Details = $"Repair auto-verified on submit for device {record.DeviceId}."
        });
        await _context.SaveChangesAsync();

        return (true, null);
    }

    public async Task<List<RepairRecord>?> GetRepairsForDeviceAsOwnerAsync(int deviceId, string ownerId)
    {
        var ownsDevice = await _context.Devices
            .AnyAsync(d => d.Id == deviceId && d.Ownerships.Any(o => o.OwnerId == ownerId && o.EndDate == null));

        if (!ownsDevice) return null; 

        return await _context.RepairRecords
            .Include(r => r.Parts)
            .Where(r => r.DeviceId == deviceId)
            .OrderByDescending(r => r.RepairDate)
            .ToListAsync();
    }

    public async Task<bool> DeviceIsOwnedByAsync(int deviceId, string ownerId)
    {
        return await _context.Devices
            .AnyAsync(d => d.Id == deviceId && d.Ownerships.Any(o => o.OwnerId == ownerId && o.EndDate == null));
    }

    public async Task<List<RepairRecord>> GetRepairsForDeviceAsTechnicianAsync(int deviceId, string technicianUserId)
    {
        return await _context.RepairRecords
            .Include(r => r.Parts)
            .Where(r => r.DeviceId == deviceId && r.TechnicianProfile.UserId == technicianUserId)
            .OrderByDescending(r => r.RepairDate)
            .ToListAsync();
    }

    public async Task<RepairRecord?> GetByIdAsync(int repairId)
    {
        return await _context.RepairRecords
            .Include(r => r.Parts)
            .Include(r => r.TechnicianProfile)
            .FirstOrDefaultAsync(r => r.Id == repairId);
    }

    public async Task<List<RepairRecord>> GetRepairsByTechnicianAsync(string technicianUserId)
    {
        return await _context.RepairRecords
            .Include(r => r.Parts)
            .Include(r => r.Device)
            .Where(r => r.TechnicianProfile.UserId == technicianUserId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}

public record CreateRepairInput(
    string ProblemDescription, string Diagnosis, string ActionTaken,
    DateTime RepairDate, DateTime? WarrantyUntil, int? CorrectsRecordId);

public record UpdateRepairInput(
    string ProblemDescription, string Diagnosis, string ActionTaken,
    DateTime RepairDate, DateTime? WarrantyUntil);