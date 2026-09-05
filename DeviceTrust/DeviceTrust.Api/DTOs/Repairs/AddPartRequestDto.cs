namespace DeviceTrust.Api.DTOs.Repairs;

public class AddPartRequestDto
{
    public string PartName { get; set; } = default!;
    public string? OldPartSerial { get; set; }
    public string? NewPartSerial { get; set; }
    public string PartType { get; set; } = default!;
    public bool IsOriginal { get; set; } = true;
    public string? Notes { get; set; }
}