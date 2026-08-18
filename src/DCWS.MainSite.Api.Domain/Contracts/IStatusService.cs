using DCWS.MainSite.Api.Domain.Models;
using DCWS.MainSite.Api.Domain.Utilities;

namespace DCWS.MainSite.Api.Domain.Contracts;

public interface IStatusService
{
    Task<ApiResponse<StatusResponse>> GetStatusAsync();
}
