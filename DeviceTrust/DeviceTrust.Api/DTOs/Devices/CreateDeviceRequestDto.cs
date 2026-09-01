using DeviceTrust.Domain.Enums;

namespace DeviceTrust.Api.DTOs.Devices;

public class CreateDeviceRequestDto
{
    public DeviceType Type { get; set; }
    public string Brand { get; set; } = default!;
    public string Model { get; set; } = default!;
    public string SerialNumber { get; set; } = default!;
    public DateTime? PurchaseDate { get; set; }
}