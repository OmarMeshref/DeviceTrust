namespace DeviceTrust.Api.DTOs.Repairs;

public class CreateRepairRequestDto
{
    public string ProblemDescription { get; set; } = default!;
    public string Diagnosis { get; set; } = default!;
    public string ActionTaken { get; set; } = default!;
    public DateTime RepairDate { get; set; }
    public DateTime? WarrantyUntil { get; set; }
    public int? CorrectsRecordId { get; set; }  
}