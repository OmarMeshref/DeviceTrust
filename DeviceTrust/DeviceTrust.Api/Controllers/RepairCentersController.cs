using DeviceTrust.Api.DTOs.RepairCenters;
using DeviceTrust.Infrastructure.Identity;
using DeviceTrust.Infrastructure.RepairCenters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DeviceTrust.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
public class RepairCentersController : ControllerBase
{
    private readonly RepairCenterService _repairCenterService;
    private readonly UserManager<ApplicationUser> _userManager;

    public RepairCentersController(RepairCenterService repairCenterService, UserManager<ApplicationUser> userManager)
    {
        _repairCenterService = repairCenterService;
        _userManager = userManager;
    }

    [HttpPost("api/repaircenters")]
    public async Task<IActionResult> Create(CreateRepairCenterRequestDto dto)
    {
        var center = await _repairCenterService.CreateAsync(dto.Name, dto.Address);
        return CreatedAtAction(nameof(GetAll), new { }, new { center.Id, center.Name });
    }

    [HttpGet("api/repaircenters")]
    public async Task<IActionResult> GetAll()
    {
        var centers = await _repairCenterService.GetAllAsync();

        var dto = centers.Select(c => new RepairCenterDto
        {
            Id = c.Id,
            Name = c.Name,
            Address = c.Address,
            IsApproved = c.IsApproved,
            CreatedAt = c.CreatedAt,
            TechnicianCount = c.Technicians.Count
        });

        return Ok(dto);
    }

    [HttpPost("api/repaircenters/{id}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var (success, error) = await _repairCenterService.ApproveAsync(id);
        return success ? Ok(new { message = "Repair center approved." }) : BadRequest(new { error });
    }

    [HttpGet("api/technicians")]
    public async Task<IActionResult> GetAllTechnicians()
    {
        var technicians = await _repairCenterService.GetAllTechniciansAsync();

        var result = new List<TechnicianDto>();
        foreach (var t in technicians)
        {
            var user = await _userManager.FindByIdAsync(t.UserId);
            result.Add(new TechnicianDto
            {
                Id = t.Id,
                UserId = t.UserId,
                Email = user?.Email ?? "(unknown)",
                FullName = user?.FullName ?? "(unknown)",
                RepairCenterId = t.RepairCenterId,
                RepairCenterName = t.RepairCenter?.Name,
                IsApproved = t.IsApproved
            });
        }

        return Ok(result);
    }

    [HttpPost("api/technicians/{id}/link")]
    public async Task<IActionResult> LinkTechnician(int id, LinkTechnicianRequestDto dto)
    {
        var (success, error) = await _repairCenterService.LinkTechnicianAsync(id, dto.RepairCenterId);
        return success ? Ok(new { message = "Technician linked." }) : BadRequest(new { error });
    }

    [HttpPost("api/technicians/{id}/unlink")]
    public async Task<IActionResult> UnlinkTechnician(int id)
    {
        var (success, error) = await _repairCenterService.UnlinkTechnicianAsync(id);
        return success ? Ok(new { message = "Technician unlinked." }) : BadRequest(new { error });
    }

    [HttpPost("api/technicians/{id}/approve")]
    public async Task<IActionResult> ApproveTechnician(int id)
    {
        var (success, error) = await _repairCenterService.ApproveTechnicianAsync(id);
        return success ? Ok(new { message = "Technician approved." }) : BadRequest(new { error });
    }
}