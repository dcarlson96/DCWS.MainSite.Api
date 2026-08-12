using DCWS.MainSite.Api.Domain.Models;

namespace DCWS.MainSite.Api.Domain.Contracts;

public interface IStatusService
{
    StatusResponse GetStatus();
}
