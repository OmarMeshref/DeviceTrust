using DeviceTrust.Domain.Entities;
using DeviceTrust.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DeviceTrust.Infrastructure.Devices;

public class DeviceService
{
    private readonly DeviceTrustDbContext _context;
    private readonly PassportIdGenerator _passportIdGenerator;

    public DeviceService(DeviceTrustDbContext context, PassportIdGenerator passportIdGenerator)
    {
        _context = context;
        _passportIdGenerator = passportIdGenerator;
    }

    public async Task<Device> CreateDeviceAsync(
        string ownerId, DeviceTrust.Domain.Enums.DeviceType type,
        string brand, string model, string serialNumber, DateTime? purchaseDate)
    {
        var publicPassportId = await _passportIdGenerator.GenerateAsync();

        var device = new Device
        {
            PublicPassportId = publicPassportId,
            Type = type,
            Brand = brand,
            Model = model,
            SerialNumber = serialNumber,
            PurchaseDate = purchaseDate,
            RegisteredAt = DateTime.UtcNow
        };

        using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

        _context.Devices.Add(device);
        await _context.SaveChangesAsync(); // need device.Id generated before creating Ownership

        _context.Ownerships.Add(new Ownership
        {
            DeviceId = device.Id,
            OwnerId = ownerId,
            StartDate = DateTime.UtcNow,
            EndDate = null
        });
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return device;
    }

    public async Task<List<Device>> GetDevicesForOwnerAsync(string ownerId)
    {
        return await _context.Devices
            .Where(d => d.Ownerships.Any(o => o.OwnerId == ownerId && o.EndDate == null))
            .OrderByDescending(d => d.RegisteredAt)
            .ToListAsync();
    }

    public async Task<(Device? device, int repairCount)> GetDeviceForOwnerAsync(int deviceId, string ownerId)
    {
        var device = await _context.Devices
            .FirstOrDefaultAsync(d => d.Id == deviceId
                && d.Ownerships.Any(o => o.OwnerId == ownerId && o.EndDate == null));

        if (device is null) return (null, 0);

        var repairCount = await _context.RepairRecords.CountAsync(r => r.DeviceId == deviceId);

        return (device, repairCount);
    }

    public async Task<Device?> GetDeviceByPublicIdAsync(string publicPassportId)
    {
        return await _context.Devices
            .FirstOrDefaultAsync(d => d.PublicPassportId == publicPassportId);
    }
}