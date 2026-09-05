using DeviceTrust.Domain.Enums;

namespace DeviceTrust.Api.DTOs.Repairs;

public class RepairDetailDto
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public RepairStatus Status { get; set; }
    public string ProblemDescription { get; set; } = default!;
    public string Diagnosis { get; set; } = default!;
    public string ActionTaken { get; set; } = default!;
    public DateTime RepairDate { get; set; }
    public DateTime? WarrantyUntil { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public int? CorrectsRecordId { get; set; }
    public List<PartDto> Parts { get; set; } = new();
}

public class PartDto
{
    public int Id { get; set; }
    public string PartName { get; set; } = default!;
    public string? OldPartSerial { get; set; }
    public string? NewPartSerial { get; set; }
    public string PartType { get; set; } = default!;
    public bool IsOriginal { get; set; }
    public string? Notes { get; set; }
}