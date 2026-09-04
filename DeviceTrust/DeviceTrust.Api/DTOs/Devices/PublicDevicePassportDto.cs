using DeviceTrust.Domain.Enums;

namespace DeviceTrust.Api.DTOs.Devices;

public class PublicDevicePassportDto
{
    public string PublicPassportId { get; set; } = default!;
    public DeviceType Type { get; set; }
    public string Brand { get; set; } = default!;
    public string Model { get; set; } = default!;
    public string MaskedSerialNumber { get; set; } = default!; 
    public DateTime RegisteredAt { get; set; }

    public int TotalRepairCount { get; set; }
    public int VerifiedRepairCount { get; set; }
    public List<PublicRepairTimelineItemDto> RepairTimeline { get; set; } = new();
}

public class PublicRepairTimelineItemDto
{
    public DateTime RepairDate { get; set; }
    public string ActionTaken { get; set; } = default!;
}