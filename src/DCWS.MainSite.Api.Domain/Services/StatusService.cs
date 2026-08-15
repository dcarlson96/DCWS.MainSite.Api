using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.Models;

namespace DCWS.MainSite.Api.Domain.Services;

public sealed class StatusService(IStatusRepository statusRepository) : IStatusService
{
    public async Task<StatusResponse> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        var status = await statusRepository.GetLatestAsync(cancellationToken);

        if (status is null)
        {
            return new StatusResponse(
                Status: "ok",
                Database: "connected",
                Id: null,
                Message: "No status record found.",
                CreatedDateUtc: null);
        }

        return new StatusResponse(
            Status: "ok",
            Database: "connected",
            Id: status.Id,
            Message: status.Message,
            CreatedDateUtc: new DateTimeOffset(status.CreatedDateUtc, TimeSpan.Zero));
    }
}
