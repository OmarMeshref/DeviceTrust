namespace DeviceTrust.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }

    public string EntityType { get; set; } = default!; 
    public int EntityId { get; set; }                   

    public string Action { get; set; } = default!;       // e.g. "Verified", "Accepted", "Created"
    public string PerformedByUserId { get; set; } = default!;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Details { get; set; }               
}