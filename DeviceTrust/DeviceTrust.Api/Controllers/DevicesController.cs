using DeviceTrust.Api.DTOs.Devices;
using DeviceTrust.Api.Extensions;
using DeviceTrust.Infrastructure.Devices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeviceTrust.Api.Controllers;

[ApiController]
[Route("api/devices")]
[Authorize(Roles = "Owner")]
public class DevicesController : ControllerBase
{
    private readonly DeviceService _deviceService;

    public DevicesController(DeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyDevices()
    {
        var ownerId = User.GetUserId();
        var devices = await _deviceService.GetDevicesForOwnerAsync(ownerId);

        var dto = devices.Select(d => new DeviceListItemDto
        {
            Id = d.Id,
            PublicPassportId = d.PublicPassportId,
            Type = d.Type,
            Brand = d.Brand,
            Model = d.Model,
            RegisteredAt = d.RegisteredAt
        });

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDevice(CreateDeviceRequestDto dto)
    {
        var ownerId = User.GetUserId();

        var device = await _deviceService.CreateDeviceAsync(
            ownerId, dto.Type, dto.Brand, dto.Model, dto.SerialNumber, dto.PurchaseDate);

        return CreatedAtAction(nameof(GetDevice), new { id = device.Id }, new
        {
            device.Id,
            device.PublicPassportId
        });
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var ownerId = User.GetUserId();
        var summary = await _deviceService.GetOwnerSummaryAsync(ownerId);
        return Ok(summary); 
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDevice(int id)
    {
        var ownerId = User.GetUserId();
        var (device, repairCount) = await _deviceService.GetDeviceForOwnerAsync(id, ownerId);

        if (device is null) return NotFound();

        var history = await _deviceService.GetOwnershipHistoryAsync(id);

        return Ok(new OwnerDeviceDetailDto
        {
            Id = device.Id,
            PublicPassportId = device.PublicPassportId,
            Type = device.Type,
            Brand = device.Brand,
            Model = device.Model,
            SerialNumber = device.SerialNumber,
            PurchaseDate = device.PurchaseDate,
            RegisteredAt = device.RegisteredAt,
            RepairCount = repairCount,
            OwnershipHistory = history.Select(o => new OwnershipPeriodDto
            {
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                IsCurrent = o.EndDate == null
            }).ToList()
        });
    }
}