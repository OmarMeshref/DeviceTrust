using DeviceTrust.Domain.Enums;

namespace DeviceTrust.Domain.Entities;

public class Device
{
    public int Id { get; set; }                         
    public string PublicPassportId { get; set; } = default!; // e.g. "DVT-A83K92", exposed publicly

    public DeviceType Type { get; set; }
    public string Brand { get; set; } = default!;
    public string Model { get; set; } = default!;
    public string SerialNumber { get; set; } = default!;   // full serial, private-only
    public DateTime? PurchaseDate { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Ownership> Ownerships { get; set; } = new List<Ownership>();
    public ICollection<RepairRecord> RepairRecords { get; set; } = new List<RepairRecord>();
    public ICollection<OwnershipTransfer> Transfers { get; set; } = new List<OwnershipTransfer>();
}