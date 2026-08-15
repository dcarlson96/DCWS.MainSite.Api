using DCWS.MainSite.Api.Domain.Models;

namespace DCWS.MainSite.Api.Domain.Contracts;

public interface IStatusRepository
{
    Task<StatusEntry?> GetLatestAsync(CancellationToken cancellationToken = default);
}
