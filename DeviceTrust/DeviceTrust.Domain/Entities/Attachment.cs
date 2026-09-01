using DeviceTrust.Domain.Enums;

namespace DeviceTrust.Domain.Entities;

public class Attachment
{
    public int Id { get; set; }

    public int RepairRecordId { get; set; }
    public RepairRecord RepairRecord { get; set; } = default!;

    public string FilePath { get; set; } = default!;
    public AttachmentType AttachmentType { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}