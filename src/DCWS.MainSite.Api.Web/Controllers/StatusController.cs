using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace DCWS.MainSite.Api.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class StatusController(IStatusService statusService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<StatusResponse>(StatusCodes.Status200OK)]
    public ActionResult<StatusResponse> GetStatus()
    {
        return Ok(statusService.GetStatus());
    }
}
