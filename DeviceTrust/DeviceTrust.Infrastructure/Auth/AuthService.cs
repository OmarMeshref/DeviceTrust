using DeviceTrust.Domain.Entities;
using DeviceTrust.Infrastructure.Data;
using DeviceTrust.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace DeviceTrust.Infrastructure.Auth;

public class AuthService
{
    private static readonly string[] AllowedSelfRegisterRoles = { "Owner", "Technician" };

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DeviceTrustDbContext _context;
    private readonly JwtTokenGenerator _jwtGenerator;

    public AuthService(UserManager<ApplicationUser> userManager, DeviceTrustDbContext context, JwtTokenGenerator jwtGenerator)
    {
        _userManager = userManager;
        _context = context;
        _jwtGenerator = jwtGenerator;
    }

    public async Task<(bool success, string? error, string? token, DateTime expiresAt, string? role)> RegisterAsync(
        string fullName, string email, string password, string role)
    {
        // Server-side role gate — the client can send anything, but only these two are ever accepted.
        // This is the single most important line in this method: it's what prevents someone
        // from POSTing {"role": "Admin"} and self-promoting.
        if (!AllowedSelfRegisterRoles.Contains(role))
            return (false, "Invalid role. Only 'Owner' or 'Technician' are allowed.", null, default, null);

        var existing = await _userManager.FindByEmailAsync(email);
        if (existing is not null)
            return (false, "Email already registered.", null, default, null);

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            EmailConfirmed = true // no email verification flow in MVP
        };

        // Use a DB transaction so ApplicationUser + (optional) TechnicianProfile succeed or fail together.
        using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            await transaction.RollbackAsync();
            return (false, string.Join(", ", createResult.Errors.Select(e => e.Description)), null, default, null);
        }

        await _userManager.AddToRoleAsync(user, role);

        if (role == "Technician")
        {
            _context.TechnicianProfiles.Add(new TechnicianProfile
            {
                UserId = user.Id,
                RepairCenterId = null,   // unlinked at registration — Admin links later
                IsApproved = false
            });
            await _context.SaveChangesAsync();
        }

        await transaction.CommitAsync();

        var (token, expiresAt) = _jwtGenerator.GenerateToken(user, role);
        return (true, null, token, expiresAt, role);
    }

    public async Task<(bool success, string? error, string? token, DateTime expiresAt, string? role)> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return (false, "Invalid email or password.", null, default, null);

        var passwordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!passwordValid)
            return (false, "Invalid email or password.", null, default, null);

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Owner"; // shouldn't happen, but fail safe rather than throw

        var (token, expiresAt) = _jwtGenerator.GenerateToken(user, role);
        return (true, null, token, expiresAt, role);
    }
}