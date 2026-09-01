using DeviceTrust.Api.DTOs.Auth;
using DeviceTrust.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;

namespace DeviceTrust.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto dto)
    {
        var (success, error, token, expiresAt, role) = await _authService.RegisterAsync(
            dto.FullName, dto.Email, dto.Password, dto.Role);

        if (!success)
            return BadRequest(new { error });

        return Ok(new AuthResponseDto { Token = token!, Email = dto.Email, Role = role!, ExpiresAt = expiresAt });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        var (success, error, token, expiresAt, role) = await _authService.LoginAsync(dto.Email, dto.Password);

        if (!success)
            return Unauthorized(new { error });

        
        return Ok(new AuthResponseDto { Token = token!, Email = dto.Email, Role = role!, ExpiresAt = expiresAt });
    }
}