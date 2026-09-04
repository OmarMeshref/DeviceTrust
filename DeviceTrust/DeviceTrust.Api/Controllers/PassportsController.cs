using DeviceTrust.Api.DTOs.Devices;
using DeviceTrust.Api.Helpers;
using DeviceTrust.Infrastructure.Devices;
using Microsoft.AspNetCore.Mvc;

namespace DeviceTrust.Api.Controllers;

[ApiController]
[Route("api/passports")]
public class PassportsController : ControllerBase
{
    private readonly DeviceService _deviceService;

    public PassportsController(DeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet("{publicId}")]
    public async Task<IActionResult> GetPassport(string publicId)
    {
        var device = await _deviceService.GetDeviceByPublicIdAsync(publicId);
        if (device is null) return NotFound();

        var dto = new PublicDevicePassportDto
        {
            PublicPassportId = device.PublicPassportId,
            Type = device.Type,
            Brand = device.Brand,
            Model = device.Model,
            MaskedSerialNumber = SerialMasker.Mask(device.SerialNumber),
            RegisteredAt = device.RegisteredAt,
            TotalRepairCount = 0,
            VerifiedRepairCount = 0
        };

        return Ok(dto);
    }
}