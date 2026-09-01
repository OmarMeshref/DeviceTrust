using DeviceTrust.Domain.Enums;

namespace DeviceTrust.Domain.Entities;

public class OwnershipTransfer
{
    public int Id { get; set; }

    public int DeviceId { get; set; }
    public Device Device { get; set; } = default!;

    public string InitiatingOwnerId { get; set; } = default!;  
    public string TargetBuyerId { get; set; } = default!;     

    public TransferStatus Status { get; set; } = TransferStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);
    public DateTime? RespondedAt { get; set; }
}