using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace DCWS.MainSite.Api.Web.Controllers;

[ApiController]
[Route("api/status")]
public sealed class StatusController(IStatusService statusService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<StatusResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<StatusResponse>> GetStatus(CancellationToken cancellationToken)
    {
        var status = await statusService.GetStatusAsync(cancellationToken);
        return Ok(status);
    }
}
