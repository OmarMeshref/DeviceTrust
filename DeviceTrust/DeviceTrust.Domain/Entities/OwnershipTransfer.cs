namespace DeviceTrust.Domain.Entities;

public class OwnershipTransfer
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public Device Device { get; set; } = default!;
}