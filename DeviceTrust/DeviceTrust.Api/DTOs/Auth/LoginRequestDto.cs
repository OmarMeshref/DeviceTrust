namespace DeviceTrust.Api.DTOs.Auth;

public class LoginRequestDto
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}