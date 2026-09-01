namespace DeviceTrust.Domain.Entities;

public class RepairPart
{
    public int Id { get; set; }

    public int RepairRecordId { get; set; }
    public RepairRecord RepairRecord { get; set; } = default!;

    public string PartName { get; set; } = default!;
    public string? OldPartSerial { get; set; }
    public string? NewPartSerial { get; set; }
    public string PartType { get; set; } = default!;
    public bool IsOriginal { get; set; } = true;
    public string? Notes { get; set; }
}