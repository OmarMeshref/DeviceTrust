namespace DeviceTrust.Api.DTOs.Devices;

public class DeviceLookupDto
{
    public int DeviceId { get; set; }
    public string PublicPassportId { get; set; } = default!;
    public string Brand { get; set; } = default!;
    public string Model { get; set; } = default!;
}