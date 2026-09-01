namespace DeviceTrust.Domain.Entities;

public class RepairCenter
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public bool IsApproved { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TechnicianProfile> Technicians { get; set; } = new List<TechnicianProfile>();
}