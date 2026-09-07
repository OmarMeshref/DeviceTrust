using DeviceTrust.Domain.Entities;
using DeviceTrust.Domain.Enums;
using DeviceTrust.Infrastructure.Data;
using DeviceTrust.Infrastructure.Repairs;
using Xunit;

namespace DeviceTrust.Tests;

public class RepairServiceTests
{
    private static async Task<(DeviceTrustDbContext context, TechnicianProfile trustedTech, Device device)> SetupTrustedScenarioAsync()
    {
        var context = TestDbContextFactory.Create();

        var device = new Device { Id = 1, PublicPassportId = "DVT-TEST01", Brand = "Test", Model = "Test", SerialNumber = "SN1", RegisteredAt = DateTime.UtcNow };
        context.Devices.Add(device);

        var center = new RepairCenter { Id = 1, Name = "TestCenter", Address = "Test Address", IsApproved = true, CreatedAt = DateTime.UtcNow };
        context.RepairCenters.Add(center);

        var tech = new TechnicianProfile { Id = 1, UserId = "tech-1", RepairCenterId = 1, IsApproved = true, CreatedAt = DateTime.UtcNow };
        context.TechnicianProfiles.Add(tech);

        await context.SaveChangesAsync();

        return (context, tech, device);
    }

    [Fact]
    public async Task CreateDraftAsync_Fails_WhenTechnicianNotApproved()
    {
        var (context, tech, device) = await SetupTrustedScenarioAsync();
        tech.IsApproved = false; // simulate a not-yet-approved technician
        await context.SaveChangesAsync();

        var service = new RepairService(context);
        var input = new CreateRepairInput("Problem", "Diagnosis", "Action", DateTime.UtcNow, null, null);

        var result = await service.CreateDraftAsync(device.Id, tech.UserId, input);

        Assert.False(result.success);
        Assert.Contains("not approved", result.error);
    }

    [Fact]
    public async Task CreateDraftAsync_Fails_WhenRepairCenterNotApproved()
    {
        var (context, tech, device) = await SetupTrustedScenarioAsync();
        var center = await context.RepairCenters.FindAsync(1);
        center!.IsApproved = false; // the center itself loses approval
        await context.SaveChangesAsync();

        var service = new RepairService(context);
        var input = new CreateRepairInput("Problem", "Diagnosis", "Action", DateTime.UtcNow, null, null);

        var result = await service.CreateDraftAsync(device.Id, tech.UserId, input);

        Assert.False(result.success);
        Assert.Contains("repair center is not approved", result.error);
    }

    [Fact]
    public async Task SubmitAsync_MarksRecordVerified_WhenTechnicianStillTrusted()
    {
        var (context, tech, device) = await SetupTrustedScenarioAsync();
        var service = new RepairService(context);
        var input = new CreateRepairInput("Problem", "Diagnosis", "Action", DateTime.UtcNow, null, null);

        var created = await service.CreateDraftAsync(device.Id, tech.UserId, input);
        Assert.True(created.success, created.error);

        var submit = await service.SubmitAsync(created.record!.Id, tech.UserId);

        Assert.True(submit.success, submit.error);
        var record = await context.RepairRecords.FindAsync(created.record.Id);
        Assert.Equal(RepairStatus.Verified, record!.Status);
        Assert.NotNull(record.VerifiedAt);
    }

    [Fact]
    public async Task SubmitAsync_Fails_WhenTechnicianTrustRevokedAfterDraftCreated()
    {
        // This is the specific rule worth proving in isolation: trust is re-checked at
        // submit time, not just at draft-creation time. A technician approved when they
        // started the draft must still be approved when they finalize it.
        var (context, tech, device) = await SetupTrustedScenarioAsync();
        var service = new RepairService(context);
        var input = new CreateRepairInput("Problem", "Diagnosis", "Action", DateTime.UtcNow, null, null);

        var created = await service.CreateDraftAsync(device.Id, tech.UserId, input);
        Assert.True(created.success, created.error);

        // Admin revokes the technician's approval between draft creation and submission.
        tech.IsApproved = false;
        await context.SaveChangesAsync();

        var submit = await service.SubmitAsync(created.record!.Id, tech.UserId);

        Assert.False(submit.success);
        Assert.Contains("Cannot submit", submit.error);

        var record = await context.RepairRecords.FindAsync(created.record.Id);
        Assert.Equal(RepairStatus.Draft, record!.Status); // must remain unverified
    }

    [Fact]
    public async Task UpdateDraftAsync_Fails_OnceRecordIsVerified()
    {
        // Proves the core trust guarantee: a Verified record becomes structurally
        // unreachable through the edit path, because GetOwnedDraftAsync filters on
        // Status == Draft directly in the query.
        var (context, tech, device) = await SetupTrustedScenarioAsync();
        var service = new RepairService(context);
        var input = new CreateRepairInput("Problem", "Diagnosis", "Action", DateTime.UtcNow, null, null);

        var created = await service.CreateDraftAsync(device.Id, tech.UserId, input);
        var submit = await service.SubmitAsync(created.record!.Id, tech.UserId);
        Assert.True(submit.success, submit.error);

        var updateInput = new UpdateRepairInput("Changed", "Changed", "Changed", DateTime.UtcNow, null);
        var update = await service.UpdateDraftAsync(created.record.Id, tech.UserId, updateInput);

        Assert.False(update.success);

        // Confirm the record's actual content was never touched.
        var record = await context.RepairRecords.FindAsync(created.record.Id);
        Assert.Equal("Problem", record!.ProblemDescription);
    }
}