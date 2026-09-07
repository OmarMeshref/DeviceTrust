// DeviceTrust.Tests/TransferServiceTests.cs
using DeviceTrust.Domain.Entities;
using DeviceTrust.Domain.Enums;
using DeviceTrust.Infrastructure.Data;
using DeviceTrust.Infrastructure.Identity;
using DeviceTrust.Infrastructure.Transfers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DeviceTrust.Tests;

public class TransferServiceTests
{
    private static UserManager<ApplicationUser> CreateUserManager(DeviceTrustDbContext context)
    {
        var store = new UserStore<ApplicationUser, IdentityRole, DeviceTrustDbContext>(context);

        // A real normalizer this time — without it, FindByEmailAsync can never match
        // anything, since NormalizedEmail is what Identity actually queries against.
        return new UserManager<ApplicationUser>(
            store,
            null!,
            new PasswordHasher<ApplicationUser>(),
            new List<IUserValidator<ApplicationUser>>(),
            new List<IPasswordValidator<ApplicationUser>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null!,
            null!);
    }

    // Creates a user the same way production registration does, so NormalizedEmail/
    // NormalizedUserName are set correctly — not hand-rolled entity construction.
    private static async Task<ApplicationUser> CreateTestUserAsync(UserManager<ApplicationUser> userManager, string email, string fullName)
    {
        var user = new ApplicationUser { UserName = email, Email = email, FullName = fullName, EmailConfirmed = true };
        var result = await userManager.CreateAsync(user, "Passw0rd!");
        if (!result.Succeeded)
            throw new InvalidOperationException("Test user setup failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        return user;
    }

    [Fact]
    public async Task CreateTransferAsync_BlocksSecondPendingTransfer_ForSameDevice()
    {
        await using var context = TestDbContextFactory.Create();
        var userManager = CreateUserManager(context);

        var owner = await CreateTestUserAsync(userManager, "owner@test.com", "Test Owner");
        var buyer = await CreateTestUserAsync(userManager, "buyer@test.com", "Test Buyer");

        var device = new Device { Id = 1, PublicPassportId = "DVT-TEST01", Brand = "Test", Model = "Test", SerialNumber = "SN1", RegisteredAt = DateTime.UtcNow };
        context.Devices.Add(device);
        context.Ownerships.Add(new Ownership { DeviceId = 1, OwnerId = owner.Id, StartDate = DateTime.UtcNow, EndDate = null });
        await context.SaveChangesAsync();

        var service = new TransferService(context, userManager);

        var first = await service.CreateTransferAsync(1, owner.Id, "buyer@test.com");
        var second = await service.CreateTransferAsync(1, owner.Id, "buyer@test.com");

        Assert.True(first.success, first.error);
        Assert.False(second.success);
        Assert.Equal("A pending transfer already exists for this device.", second.error);
    }

    [Fact]
    public async Task CreateTransferAsync_RejectsTransfer_WhenBuyerNotRegistered()
    {
        await using var context = TestDbContextFactory.Create();
        var userManager = CreateUserManager(context);

        var owner = await CreateTestUserAsync(userManager, "owner@test.com", "Test Owner");

        var device = new Device { Id = 1, PublicPassportId = "DVT-TEST02", Brand = "Test", Model = "Test", SerialNumber = "SN2", RegisteredAt = DateTime.UtcNow };
        context.Devices.Add(device);
        context.Ownerships.Add(new Ownership { DeviceId = 1, OwnerId = owner.Id, StartDate = DateTime.UtcNow, EndDate = null });
        await context.SaveChangesAsync();

        var service = new TransferService(context, userManager);

        var result = await service.CreateTransferAsync(1, owner.Id, "nobody@test.com");

        Assert.False(result.success);
        Assert.Contains("must have a DeviceTrust account", result.error);
    }

    [Fact]
    public async Task AcceptTransferAsync_AtomicallyMovesOwnership_AndClosesOldOwnership()
    {
        await using var context = TestDbContextFactory.Create();
        var userManager = CreateUserManager(context);

        var owner = await CreateTestUserAsync(userManager, "owner@test.com", "Test Owner");
        var buyer = await CreateTestUserAsync(userManager, "buyer@test.com", "Test Buyer");

        var device = new Device { Id = 1, PublicPassportId = "DVT-TEST03", Brand = "Test", Model = "Test", SerialNumber = "SN3", RegisteredAt = DateTime.UtcNow };
        context.Devices.Add(device);
        context.Ownerships.Add(new Ownership { DeviceId = 1, OwnerId = owner.Id, StartDate = DateTime.UtcNow, EndDate = null });
        await context.SaveChangesAsync();

        var service = new TransferService(context, userManager);
        var created = await service.CreateTransferAsync(1, owner.Id, "buyer@test.com");
        Assert.True(created.success, created.error);

        var accept = await service.AcceptTransferAsync(created.transfer!.Id, buyer.Id);
        Assert.True(accept.success, accept.error);

        var ownerships = await context.Ownerships.Where(o => o.DeviceId == 1).ToListAsync();
        var active = ownerships.Single(o => o.EndDate == null);
        var closed = ownerships.Single(o => o.EndDate != null);

        Assert.Equal(buyer.Id, active.OwnerId);
        Assert.Equal(owner.Id, closed.OwnerId);

        var transfer = await context.OwnershipTransfers.FindAsync(created.transfer.Id);
        Assert.Equal(TransferStatus.Accepted, transfer!.Status);
    }
}