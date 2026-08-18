using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.Models;
using DCWS.MainSite.Api.Domain.Services;

namespace DCWS.MainSite.Api.Tests.Services;

public sealed class StatusServiceTests
{
    private sealed class FakeStatusRepository(StatusTest? entry) : IStatusRepository
    {
        public Task<StatusTest?> GetLatestAsync()
        {
            return Task.FromResult(entry);
        }
    }

    [Fact]
    public async Task GetStatusAsync_ReturnsMappedResponse_WhenEntryExists()
    {
        var createdDateUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var repository = new FakeStatusRepository(new StatusTest
        {
            Id = 42,
            Message = "hello",
            CreatedDateUtc = createdDateUtc
        });
        var service = new StatusService(repository);

        var result = await service.GetStatusAsync();

        Assert.True(result.WasSuccessful);
        Assert.NotNull(result.Item);
        Assert.Equal("ok", result.Item!.Status);
        Assert.Equal("connected", result.Item.Database);
        Assert.Equal(42, result.Item.Id);
        Assert.Equal("hello", result.Item.Message);
        Assert.Equal(new DateTimeOffset(createdDateUtc, TimeSpan.Zero), result.Item.CreatedDateUtc);
    }

    [Fact]
    public async Task GetStatusAsync_ReturnsNoRecordMessage_WhenEntryMissing()
    {
        var repository = new FakeStatusRepository(entry: null);
        var service = new StatusService(repository);

        var result = await service.GetStatusAsync();

        Assert.True(result.WasSuccessful);
        Assert.NotNull(result.Item);
        Assert.Equal("ok", result.Item!.Status);
        Assert.Equal("connected", result.Item.Database);
        Assert.Null(result.Item.Id);
        Assert.Equal("No status record found.", result.Item.Message);
        Assert.Null(result.Item.CreatedDateUtc);
    }
}
