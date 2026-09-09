using DeviceTrust.Api.DTOs.Transfers;
using DeviceTrust.Api.Extensions;
using DeviceTrust.Infrastructure.Transfers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeviceTrust.Api.Controllers;

[ApiController]
[Authorize] 
public class TransfersController : ControllerBase
{
    private readonly TransferService _transferService;

    public TransfersController(TransferService transferService)
    {
        _transferService = transferService;
    }

    [HttpPost("api/devices/{deviceId}/transfers")]
    public async Task<IActionResult> Create(int deviceId, CreateTransferRequestDto dto)
    {
        var ownerId = User.GetUserId();
        var (success, error, transfer) = await _transferService.CreateTransferAsync(deviceId, ownerId, dto.BuyerEmail);

        if (!success) return BadRequest(new { error });

        return CreatedAtAction(nameof(GetPending), new { }, new { transfer!.Id, transfer.Status });
    }

    [HttpGet("api/transfers/pending")]
    public async Task<IActionResult> GetPending()
    {
        var buyerId = User.GetUserId();
        var transfers = await _transferService.GetPendingTransfersForBuyerAsync(buyerId);

        var dto = transfers.Select(t => new TransferDto
        {
            Id = t.Id,
            DeviceId = t.DeviceId,
            DevicePublicPassportId = t.Device.PublicPassportId,
            DeviceBrand = t.Device.Brand,
            DeviceModel = t.Device.Model,
            Status = t.Status,
            CreatedAt = t.CreatedAt,
            ExpiresAt = t.ExpiresAt,
            RespondedAt = t.RespondedAt,
            CallerRole = "Buyer"
        });

        return Ok(dto);
    }

    [HttpPost("api/transfers/{id}/accept")]
    public async Task<IActionResult> Accept(int id)
    {
        var buyerId = User.GetUserId();
        var (success, error) = await _transferService.AcceptTransferAsync(id, buyerId);

        return success ? Ok(new { message = "Transfer accepted." }) : BadRequest(new { error });
    }

    [HttpPost("api/transfers/{id}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        var buyerId = User.GetUserId();
        var (success, error) = await _transferService.RejectTransferAsync(id, buyerId);

        return success ? Ok(new { message = "Transfer rejected." }) : BadRequest(new { error });
    }

    [HttpPost("api/transfers/{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var ownerId = User.GetUserId();
        var (success, error) = await _transferService.CancelTransferAsync(id, ownerId);

        return success ? Ok(new { message = "Transfer cancelled." }) : BadRequest(new { error });
    }

    [HttpGet("api/transfers/history")]
    public async Task<IActionResult> GetHistory()
    {
        var userId = User.GetUserId();
        var transfers = await _transferService.GetTransferHistoryAsync(userId);

        var dto = transfers.Select(t => new TransferDto
        {
            Id = t.Id,
            DeviceId = t.DeviceId,
            DevicePublicPassportId = t.Device.PublicPassportId,
            DeviceBrand = t.Device.Brand,
            DeviceModel = t.Device.Model,
            Status = t.Status,
            CreatedAt = t.CreatedAt,
            ExpiresAt = t.ExpiresAt,
            RespondedAt = t.RespondedAt,
            CallerRole = t.InitiatingOwnerId == userId ? "Seller" : "Buyer"
        });

        return Ok(dto);
    }
}