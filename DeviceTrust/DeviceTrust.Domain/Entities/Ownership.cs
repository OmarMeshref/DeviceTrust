namespace DeviceTrust.Domain.Entities;

public class Ownership
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public Device Device { get; set; } = default!;

    public string OwnerId { get; set; } = default!; 

    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }      
}