using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.Models;
using DCWS.MainSite.Api.Domain.Utilities;

namespace DCWS.MainSite.Api.Domain.Services;

public sealed class StatusService(IStatusRepository statusRepository) : IStatusService
{
    public async Task<ApiResponse<StatusResponse>> GetStatusAsync()
    {
        var status = await statusRepository.GetLatestAsync();

        if (status is null)
        {
            return new ApiResponse<StatusResponse>
            {
                WasSuccessful = true,
                Item = new StatusResponse(
                    Status: "ok",
                    Database: "connected",
                    Id: null,
                    Message: "No status record found.",
                    CreatedDateUtc: null)
            };
        }

        return new ApiResponse<StatusResponse>
        {
            WasSuccessful = true,
            Item = new StatusResponse(
                Status: "ok",
                Database: "connected",
                Id: status.Id,
                Message: status.Message,
                CreatedDateUtc: new DateTimeOffset(status.CreatedDateUtc, TimeSpan.Zero))
        };
    }
}
