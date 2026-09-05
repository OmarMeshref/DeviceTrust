using DeviceTrust.Api.DTOs.Devices;
using DeviceTrust.Infrastructure.Devices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeviceTrust.Api.Controllers;

[ApiController]
[Authorize(Roles = "Technician")]
[Route("api/technicians/devices")]
public class TechnicianLookupController : ControllerBase
{
    private readonly DeviceService _deviceService;

    public TechnicianLookupController(DeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet("lookup/{publicId}")]
    public async Task<IActionResult> Lookup(string publicId)
    {
        var device = await _deviceService.GetDeviceByPublicIdForTechnicianAsync(publicId);
        if (device is null) return NotFound();

        return Ok(new DeviceLookupDto
        {
            DeviceId = device.Id,
            PublicPassportId = device.PublicPassportId,
            Brand = device.Brand,
            Model = device.Model
        });
    }
}