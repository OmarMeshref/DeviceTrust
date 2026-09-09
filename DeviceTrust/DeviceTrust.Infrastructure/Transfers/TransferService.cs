using DeviceTrust.Domain.Entities;
using DeviceTrust.Domain.Enums;
using DeviceTrust.Infrastructure.Data;
using DeviceTrust.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DeviceTrust.Infrastructure.Transfers;

public class TransferService
{
    private readonly DeviceTrustDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TransferService(DeviceTrustDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<(bool success, string? error, OwnershipTransfer? transfer)> CreateTransferAsync(
        int deviceId, string initiatingOwnerId, string buyerEmail)
    {
        var device = await _context.Devices
            .FirstOrDefaultAsync(d => d.Id == deviceId
                && d.Ownerships.Any(o => o.OwnerId == initiatingOwnerId && o.EndDate == null));

        if (device is null)
            return (false, "Device not found or you are not the current owner.", null);

        var buyer = await _userManager.FindByEmailAsync(buyerEmail);
        if (buyer is null)
            return (false, "No registered user found with that email. The buyer must have a DeviceTrust account.", null);

        if (buyer.Id == initiatingOwnerId)
            return (false, "You cannot transfer a device to yourself.", null);


        var existingPending = await _context.OwnershipTransfers
            .AnyAsync(t => t.DeviceId == deviceId && t.Status == TransferStatus.Pending);

        if (existingPending)
            return (false, "A pending transfer already exists for this device.", null);

        var transfer = new OwnershipTransfer
        {
            DeviceId = deviceId,
            InitiatingOwnerId = initiatingOwnerId,
            TargetBuyerId = buyer.Id,
            Status = TransferStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        try
        {
            _context.OwnershipTransfers.Add(transfer);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {

            return (false, "A pending transfer already exists for this device.", null);
        }

        return (true, null, transfer);
    }

    public async Task<List<OwnershipTransfer>> GetPendingTransfersForBuyerAsync(string buyerId)
    {
        await ExpireStaleTransfersAsync(buyerId);

        return await _context.OwnershipTransfers
            .Include(t => t.Device)
            .Where(t => t.TargetBuyerId == buyerId && t.Status == TransferStatus.Pending)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<(bool success, string? error)> AcceptTransferAsync(int transferId, string buyerId)
    {
        var transfer = await _context.OwnershipTransfers
            .FirstOrDefaultAsync(t => t.Id == transferId && t.TargetBuyerId == buyerId);

        if (transfer is null)
            return (false, "Transfer not found.");

        if (await TryExpireIfStale(transfer))
            return (false, "This transfer has expired.");

        if (transfer.Status != TransferStatus.Pending)
            return (false, $"Transfer is not pending (current status: {transfer.Status}).");


        using IDbContextTransaction dbTransaction = await _context.Database.BeginTransactionAsync();

        var currentOwnership = await _context.Ownerships
            .FirstOrDefaultAsync(o => o.DeviceId == transfer.DeviceId && o.EndDate == null);

        if (currentOwnership is null)
        {
            await dbTransaction.RollbackAsync();
            return (false, "No active ownership found for this device — data integrity issue.");
        }

        currentOwnership.EndDate = DateTime.UtcNow;

        _context.Ownerships.Add(new Ownership
        {
            DeviceId = transfer.DeviceId,
            OwnerId = buyerId,
            StartDate = DateTime.UtcNow,
            EndDate = null
        });

        transfer.Status = TransferStatus.Accepted;
        transfer.RespondedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await dbTransaction.CommitAsync();

        return (true, null);
    }

    public async Task<(bool success, string? error)> RejectTransferAsync(int transferId, string buyerId)
    {
        var transfer = await _context.OwnershipTransfers
            .FirstOrDefaultAsync(t => t.Id == transferId && t.TargetBuyerId == buyerId);

        if (transfer is null) return (false, "Transfer not found.");
        if (await TryExpireIfStale(transfer)) return (false, "This transfer has expired.");
        if (transfer.Status != TransferStatus.Pending)
            return (false, $"Transfer is not pending (current status: {transfer.Status}).");

        transfer.Status = TransferStatus.Rejected;
        transfer.RespondedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool success, string? error)> CancelTransferAsync(int transferId, string ownerId)
    {
        var transfer = await _context.OwnershipTransfers
            .FirstOrDefaultAsync(t => t.Id == transferId && t.InitiatingOwnerId == ownerId);

        if (transfer is null) return (false, "Transfer not found.");
        if (transfer.Status != TransferStatus.Pending)
            return (false, $"Transfer is not pending (current status: {transfer.Status}).");

        transfer.Status = TransferStatus.Cancelled;
        transfer.RespondedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return (true, null);
    }


    private async Task<bool> TryExpireIfStale(OwnershipTransfer transfer)
    {
        if (transfer.Status == TransferStatus.Pending && transfer.ExpiresAt < DateTime.UtcNow)
        {
            transfer.Status = TransferStatus.Expired;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    private async Task ExpireStaleTransfersAsync(string buyerId)
    {
        var stale = await _context.OwnershipTransfers
            .Where(t => t.TargetBuyerId == buyerId
                && t.Status == TransferStatus.Pending
                && t.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();

        foreach (var t in stale) t.Status = TransferStatus.Expired;
        if (stale.Count > 0) await _context.SaveChangesAsync();
    }

    public async Task<List<OwnershipTransfer>> GetTransferHistoryAsync(string userId)
    {
        return await _context.OwnershipTransfers
            .Include(t => t.Device)
            .Where(t => (t.InitiatingOwnerId == userId || t.TargetBuyerId == userId)
                        && t.Status != TransferStatus.Pending)
            .OrderByDescending(t => t.RespondedAt ?? t.CreatedAt)
            .ToListAsync();
    }
}