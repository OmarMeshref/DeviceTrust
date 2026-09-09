namespace DeviceTrust.Api.DTOs.Auth;

public class UserProfileDto
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
}