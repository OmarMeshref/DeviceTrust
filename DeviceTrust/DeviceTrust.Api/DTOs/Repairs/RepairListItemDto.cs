using DeviceTrust.Domain.Enums;

namespace DeviceTrust.Api.DTOs.Repairs;

public class RepairListItemDto
{
    public int Id { get; set; }
    public string DevicePublicPassportId { get; set; } = default!;
    public string DeviceBrand { get; set; } = default!;
    public string DeviceModel { get; set; } = default!;
    public RepairStatus Status { get; set; }
    public DateTime RepairDate { get; set; }
    public DateTime CreatedAt { get; set; }
}