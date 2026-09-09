using DeviceTrust.Domain.Enums;

namespace DeviceTrust.Api.DTOs.Transfers;

public class TransferDto
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public string DevicePublicPassportId { get; set; } = default!;
    public string DeviceBrand { get; set; } = default!;
    public string DeviceModel { get; set; } = default!;
    public TransferStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public string CallerRole { get; set; } = default!; 
}