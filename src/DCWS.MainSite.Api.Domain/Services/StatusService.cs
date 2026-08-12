using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.Models;

namespace DCWS.MainSite.Api.Domain.Services;

public sealed class StatusService : IStatusService
{
    public StatusResponse GetStatus()
    {
        return new StatusResponse(
            Message: "DC Web Systems API is running.",
            Status: "OK",
            TimestampUtc: DateTimeOffset.UtcNow);
    }
}
