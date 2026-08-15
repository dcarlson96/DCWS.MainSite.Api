using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.Models;
using DCWS.MainSite.Api.Domain.Services;

namespace DCWS.MainSite.Api.Tests.Services;

public sealed class StatusServiceTests
{
    private sealed class FakeStatusRepository(StatusEntry? entry) : IStatusRepository
    {
        public Task<StatusEntry?> GetLatestAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(entry);
        }
    }

    [Fact]
    public async Task GetStatusAsync_ReturnsMappedResponse_WhenEntryExists()
    {
        var createdDateUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var repository = new FakeStatusRepository(new StatusEntry
        {
            Id = 42,
            Message = "hello",
            CreatedDateUtc = createdDateUtc
        });
        var service = new StatusService(repository);

        var result = await service.GetStatusAsync();

        Assert.Equal("ok", result.Status);
        Assert.Equal("connected", result.Database);
        Assert.Equal(42, result.Id);
        Assert.Equal("hello", result.Message);
        Assert.Equal(new DateTimeOffset(createdDateUtc, TimeSpan.Zero), result.CreatedDateUtc);
    }

    [Fact]
    public async Task GetStatusAsync_ReturnsNoRecordMessage_WhenEntryMissing()
    {
        var repository = new FakeStatusRepository(entry: null);
        var service = new StatusService(repository);

        var result = await service.GetStatusAsync();

        Assert.Equal("ok", result.Status);
        Assert.Equal("connected", result.Database);
        Assert.Null(result.Id);
        Assert.Equal("No status record found.", result.Message);
        Assert.Null(result.CreatedDateUtc);
    }
}
