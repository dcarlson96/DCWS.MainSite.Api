using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.ExternalTypes;
using DCWS.MainSite.Api.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace DCWS.MainSite.Api.Web.Controllers;

[ApiController]
[Route("api/address")]
public sealed class AddressController(IAddressService addressService) : ControllerBase
{
    [HttpGet("lookup")]
    public Task<ApiResponse<AddressLookupResponse>> Lookup([FromQuery] AddressLookupRequest request)
    {
        return addressService.LookupAsync(request);
    }
}
