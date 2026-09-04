namespace DeviceTrust.Api.DTOs.RepairCenters;

public class TechnicianDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public int? RepairCenterId { get; set; }
    public string? RepairCenterName { get; set; }
    public bool IsApproved { get; set; }
}