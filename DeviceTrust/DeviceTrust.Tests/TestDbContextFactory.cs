using DeviceTrust.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DeviceTrust.Tests;

public static class TestDbContextFactory
{
    public static DeviceTrustDbContext Create()
    {
        var options = new DbContextOptionsBuilder<DeviceTrustDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new DeviceTrustDbContext(options);
    }
}