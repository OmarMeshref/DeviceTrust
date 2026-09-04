namespace DeviceTrust.Api.DTOs.RepairCenters;

public class CreateRepairCenterRequestDto
{
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
}