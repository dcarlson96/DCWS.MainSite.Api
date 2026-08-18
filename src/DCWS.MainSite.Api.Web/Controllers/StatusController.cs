using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.Models;
using DCWS.MainSite.Api.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace DCWS.MainSite.Api.Web.Controllers;

[ApiController]
[Route("api/status")]
public sealed class StatusController(IStatusService statusService) : ControllerBase
{
    [HttpGet("get")]
    public Task<ApiResponse<StatusResponse>> GetStatus()
    {
        return statusService.GetStatusAsync();
    }
}
