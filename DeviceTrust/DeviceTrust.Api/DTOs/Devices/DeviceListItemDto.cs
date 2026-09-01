using DeviceTrust.Domain.Enums;

namespace DeviceTrust.Api.DTOs.Devices;

public class DeviceListItemDto
{
    public int Id { get; set; }
    public string PublicPassportId { get; set; } = default!;
    public DeviceType Type { get; set; }
    public string Brand { get; set; } = default!;
    public string Model { get; set; } = default!;
    public DateTime RegisteredAt { get; set; }
}