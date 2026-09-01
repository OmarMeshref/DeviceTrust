namespace DeviceTrust.Domain.Entities;

public class TechnicianProfile
{
    public int Id { get; set; }
    public string UserId { get; set; } = default!;  

    public int? RepairCenterId { get; set; }          
    public RepairCenter? RepairCenter { get; set; }

    public bool IsApproved { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<RepairRecord> RepairRecords { get; set; } = new List<RepairRecord>();
}