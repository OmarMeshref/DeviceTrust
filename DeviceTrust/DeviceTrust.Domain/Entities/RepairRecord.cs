using DeviceTrust.Domain.Enums;
using System.Net.Mail;

namespace DeviceTrust.Domain.Entities;

public class RepairRecord
{
    public int Id { get; set; }

    public int DeviceId { get; set; }
    public Device Device { get; set; } = default!;

    public int TechnicianProfileId { get; set; }
    public TechnicianProfile TechnicianProfile { get; set; } = default!;

    public RecordType RecordType { get; set; } = RecordType.Repair;
    public RepairStatus Status { get; set; } = RepairStatus.Draft;

    public string ProblemDescription { get; set; } = default!;
    public string Diagnosis { get; set; } = default!;
    public string ActionTaken { get; set; } = default!;

    public DateTime RepairDate { get; set; } = DateTime.UtcNow;
    public DateTime? WarrantyUntil { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? VerifiedAt { get; set; }  

    public int? CorrectsRecordId { get; set; }
    public RepairRecord? CorrectsRecord { get; set; }

    public ICollection<RepairPart> Parts { get; set; } = new List<RepairPart>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}