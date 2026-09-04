namespace DeviceTrust.Api.DTOs.RepairCenters;

public class RepairCenterDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TechnicianCount { get; set; }
}