using DCWS.MainSite.Api.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace DCWS.MainSite.Api.Tests.Services;

public sealed class StatusServiceTests
{
    private static StatusService CreateService(string? connectionString = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString
            })
            .Build();

        return new StatusService(configuration);
    }

    [Fact]
    public async Task GetStatusAsync_ThrowsWhenConnectionStringMissing()
    {
        var service = CreateService(connectionString: null);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetStatusAsync());
    }
}
