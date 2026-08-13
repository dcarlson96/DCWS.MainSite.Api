using DCWS.MainSite.Api.Domain.Models;

namespace DCWS.MainSite.Api.Domain.Contracts;

public interface IStatusService
{
    Task<StatusResponse> GetStatusAsync(CancellationToken cancellationToken = default);
}
