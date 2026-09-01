using Microsoft.AspNetCore.Identity;

namespace DeviceTrust.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = default!;
}