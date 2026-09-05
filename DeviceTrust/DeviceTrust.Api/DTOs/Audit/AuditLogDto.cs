namespace DeviceTrust.Api.DTOs.Audit;

public class AuditLogDto
{
    public int Id { get; set; }
    public string EntityType { get; set; } = default!;
    public int EntityId { get; set; }
    public string Action { get; set; } = default!;
    public string PerformedByUserId { get; set; } = default!;
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }
}