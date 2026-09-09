// DeviceTrust.Api/Controllers/ProfileController.cs
using DeviceTrust.Api.DTOs.Auth;
using DeviceTrust.Api.Extensions;
using DeviceTrust.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DeviceTrust.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.GetUserId();
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new UserProfileDto
        {
            FullName = user.FullName,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? "Unknown"
        });
    }
}