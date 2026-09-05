namespace DeviceTrust.Api.DTOs.Repairs;

public class UpdateRepairRequestDto
{
    public string ProblemDescription { get; set; } = default!;
    public string Diagnosis { get; set; } = default!;
    public string ActionTaken { get; set; } = default!;
    public DateTime RepairDate { get; set; }
    public DateTime? WarrantyUntil { get; set; }
}