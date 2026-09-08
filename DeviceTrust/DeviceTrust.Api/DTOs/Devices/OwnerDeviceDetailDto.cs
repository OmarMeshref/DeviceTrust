using DeviceTrust.Domain.Enums;

namespace DeviceTrust.Api.DTOs.Devices;

public class OwnerDeviceDetailDto
{
    public int Id { get; set; }
    public string PublicPassportId { get; set; } = default!;
    public DeviceType Type { get; set; }
    public string Brand { get; set; } = default!;
    public string Model { get; set; } = default!;
    public string SerialNumber { get; set; } = default!;
    public DateTime? PurchaseDate { get; set; }
    public DateTime RegisteredAt { get; set; }
    public int RepairCount { get; set; }
    public List<OwnershipPeriodDto> OwnershipHistory { get; set; } = new();
}

public class OwnershipPeriodDto
{
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
}