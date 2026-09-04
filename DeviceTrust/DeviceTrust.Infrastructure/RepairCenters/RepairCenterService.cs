using DeviceTrust.Domain.Entities;
using DeviceTrust.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeviceTrust.Infrastructure.RepairCenters;

public class RepairCenterService
{
    private readonly DeviceTrustDbContext _context;

    public RepairCenterService(DeviceTrustDbContext context)
    {
        _context = context;
    }

    public async Task<RepairCenter> CreateAsync(string name, string address)
    {
        var center = new RepairCenter
        {
            Name = name,
            Address = address,
            IsApproved = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.RepairCenters.Add(center);
        await _context.SaveChangesAsync();
        return center;
    }

    public async Task<List<RepairCenter>> GetAllAsync()
    {
        return await _context.RepairCenters
            .Include(rc => rc.Technicians)
            .OrderByDescending(rc => rc.CreatedAt)
            .ToListAsync();
    }

    public async Task<(bool success, string? error)> ApproveAsync(int id)
    {
        var center = await _context.RepairCenters.FindAsync(id);
        if (center is null) return (false, "Repair center not found.");

        center.IsApproved = true;
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool success, string? error)> LinkTechnicianAsync(int technicianProfileId, int repairCenterId)
    {
        var technician = await _context.TechnicianProfiles.FindAsync(technicianProfileId);
        if (technician is null) return (false, "Technician profile not found.");

        var center = await _context.RepairCenters.FindAsync(repairCenterId);
        if (center is null) return (false, "Repair center not found.");

        technician.RepairCenterId = repairCenterId;
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool success, string? error)> UnlinkTechnicianAsync(int technicianProfileId)
    {
        var technician = await _context.TechnicianProfiles.FindAsync(technicianProfileId);
        if (technician is null) return (false, "Technician profile not found.");

        technician.RepairCenterId = null;
        technician.IsApproved = false;
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool success, string? error)> ApproveTechnicianAsync(int technicianProfileId)
    {
        var technician = await _context.TechnicianProfiles.FindAsync(technicianProfileId);
        if (technician is null) return (false, "Technician profile not found.");

        if (technician.RepairCenterId is null)
            return (false, "Cannot approve a technician who is not linked to a repair center.");

        technician.IsApproved = true;
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<List<TechnicianProfile>> GetAllTechniciansAsync()
    {
        return await _context.TechnicianProfiles
            .Include(t => t.RepairCenter)
            .ToListAsync();
    }
}