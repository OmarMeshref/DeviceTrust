using DeviceTrust.Api.DTOs.Repairs;
using DeviceTrust.Api.Extensions;
using DeviceTrust.Domain.Entities;
using DeviceTrust.Infrastructure.Repairs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeviceTrust.Api.Controllers;

[ApiController]
[Authorize]
public class RepairsController : ControllerBase
{
    private readonly RepairService _repairService;

    public RepairsController(RepairService repairService)
    {
        _repairService = repairService;
    }

    [HttpPost("api/devices/{deviceId}/repairs")]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> Create(int deviceId, CreateRepairRequestDto dto)
    {
        var technicianUserId = User.GetUserId();

        var input = new CreateRepairInput(
            dto.ProblemDescription, dto.Diagnosis, dto.ActionTaken,
            dto.RepairDate, dto.WarrantyUntil, dto.CorrectsRecordId);

        var (success, error, record) = await _repairService.CreateDraftAsync(deviceId, technicianUserId, input);

        if (!success) return BadRequest(new { error });

        return CreatedAtAction(nameof(GetById), new { id = record!.Id }, new { record.Id, record.Status });
    }

    [HttpPatch("api/repairs/{id}")]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> Update(int id, UpdateRepairRequestDto dto)
    {
        var technicianUserId = User.GetUserId();
        var input = new UpdateRepairInput(dto.ProblemDescription, dto.Diagnosis, dto.ActionTaken, dto.RepairDate, dto.WarrantyUntil);

        var (success, error) = await _repairService.UpdateDraftAsync(id, technicianUserId, input);
        return success ? Ok(new { message = "Draft updated." }) : BadRequest(new { error });
    }

    [HttpPost("api/repairs/{id}/parts")]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> AddPart(int id, AddPartRequestDto dto)
    {
        var technicianUserId = User.GetUserId();

        var part = new RepairPart
        {
            PartName = dto.PartName,
            OldPartSerial = dto.OldPartSerial,
            NewPartSerial = dto.NewPartSerial,
            PartType = dto.PartType,
            IsOriginal = dto.IsOriginal,
            Notes = dto.Notes
        };

        var (success, error) = await _repairService.AddPartAsync(id, technicianUserId, part);
        return success ? Ok(new { message = "Part added." }) : BadRequest(new { error });
    }

    [HttpPost("api/repairs/{id}/submit")]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> Submit(int id)
    {
        var technicianUserId = User.GetUserId();
        var (success, error) = await _repairService.SubmitAsync(id, technicianUserId);
        return success ? Ok(new { message = "Repair submitted and verified." }) : BadRequest(new { error });
    }

    [HttpGet("api/devices/{deviceId}/repairs")]
    public async Task<IActionResult> GetForDevice(int deviceId)
    {
        var userId = User.GetUserId();

        if (User.IsInRole("Owner"))
        {
            var records = await _repairService.GetRepairsForDeviceAsOwnerAsync(deviceId, userId);
            if (records is null) return NotFound();
            return Ok(records.Select(MapToDetailDto));
        }

        if (User.IsInRole("Technician"))
        {
            var records = await _repairService.GetRepairsForDeviceAsTechnicianAsync(deviceId, userId);
            return Ok(records.Select(MapToDetailDto));
        }

        return Forbid();
    }

    [HttpGet("api/repairs/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.GetUserId();
        var record = await _repairService.GetByIdAsync(id);
        if (record is null) return NotFound();

        var isOwner = User.IsInRole("Owner") &&
            await _repairService.DeviceIsOwnedByAsync(record.DeviceId, userId);
        var isAuthoringTechnician = User.IsInRole("Technician") &&
            record.TechnicianProfile.UserId == userId;

        if (!isOwner && !isAuthoringTechnician) return NotFound(); 

        return Ok(MapToDetailDto(record));
    }

    private static RepairDetailDto MapToDetailDto(RepairRecord r) => new()
    {
        Id = r.Id,
        DeviceId = r.DeviceId,
        Status = r.Status,
        ProblemDescription = r.ProblemDescription,
        Diagnosis = r.Diagnosis,
        ActionTaken = r.ActionTaken,
        RepairDate = r.RepairDate,
        WarrantyUntil = r.WarrantyUntil,
        CreatedAt = r.CreatedAt,
        VerifiedAt = r.VerifiedAt,
        CorrectsRecordId = r.CorrectsRecordId,
        Parts = r.Parts.Select(p => new PartDto
        {
            Id = p.Id,
            PartName = p.PartName,
            OldPartSerial = p.OldPartSerial,
            NewPartSerial = p.NewPartSerial,
            PartType = p.PartType,
            IsOriginal = p.IsOriginal,
            Notes = p.Notes
        }).ToList()
    };

    [HttpGet("api/technicians/repairs")]
    [Authorize(Roles = "Technician")]
    public async Task<IActionResult> GetMyRepairs()
    {
        var technicianUserId = User.GetUserId();
        var records = await _repairService.GetRepairsByTechnicianAsync(technicianUserId);

        var dto = records.Select(r => new RepairListItemDto
        {
            Id = r.Id,
            DevicePublicPassportId = r.Device.PublicPassportId,
            DeviceBrand = r.Device.Brand,
            DeviceModel = r.Device.Model,
            Status = r.Status,
            RepairDate = r.RepairDate,
            CreatedAt = r.CreatedAt
        });

        return Ok(dto);
    }
}