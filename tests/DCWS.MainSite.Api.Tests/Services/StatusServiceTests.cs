using DCWS.MainSite.Api.Domain.Services;

namespace DCWS.MainSite.Api.Tests.Services;

public sealed class StatusServiceTests
{
    private readonly StatusService _service = new();

    [Fact]
    public void GetStatus_ReturnsNonNullResponse()
    {
        var response = _service.GetStatus();

        Assert.NotNull(response);
    }

    [Fact]
    public void GetStatus_ReturnsOkStatus()
    {
        var response = _service.GetStatus();

        Assert.Equal("OK", response.Status);
    }

    [Fact]
    public void GetStatus_ReturnsExpectedMessage()
    {
        var response = _service.GetStatus();

        Assert.Equal("DC Web Systems API is running.", response.Message);
    }
}
