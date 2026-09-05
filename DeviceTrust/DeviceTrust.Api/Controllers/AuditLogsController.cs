using DeviceTrust.Api.DTOs.Audit;
using DeviceTrust.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeviceTrust.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/audit-logs")]
public class AuditLogsController : ControllerBase
{
    private readonly DeviceTrustDbContext _context;

    public AuditLogsController(DeviceTrustDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? entityType, [FromQuery] int? entityId)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(entityType))
            query = query.Where(a => a.EntityType == entityType);

        if (entityId.HasValue)
            query = query.Where(a => a.EntityId == entityId.Value);

        var logs = await query
            .OrderByDescending(a => a.Timestamp)
            .Select(a => new AuditLogDto
            {
                Id = a.Id,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                Action = a.Action,
                PerformedByUserId = a.PerformedByUserId,
                Timestamp = a.Timestamp,
                Details = a.Details
            })
            .ToListAsync();

        return Ok(logs);
    }
}