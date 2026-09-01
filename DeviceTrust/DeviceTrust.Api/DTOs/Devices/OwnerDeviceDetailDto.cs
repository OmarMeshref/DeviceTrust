using DeviceTrust.Domain.Enums;

namespace DeviceTrust.Api.DTOs.Devices;

public class OwnerDeviceDetailDto
{
    public int Id { get; set; }
    public string PublicPassportId { get; set; } = default!;
    public DeviceType Type { get; set; }
    public string Brand { get; set; } = default!;
    public string Model { get; set; } = default!;
    public string SerialNumber { get; set; } = default!;  // full serial — private DTO, owner is authorized to see it
    public DateTime? PurchaseDate { get; set; }
    public DateTime RegisteredAt { get; set; }
    public int RepairCount { get; set; }
}