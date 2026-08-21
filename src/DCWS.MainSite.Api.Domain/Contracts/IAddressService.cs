using DCWS.MainSite.Api.Domain.ExternalTypes;
using DCWS.MainSite.Api.Domain.Utilities;

namespace DCWS.MainSite.Api.Domain.Contracts;

public interface IAddressService
{
    Task<ApiResponse<AddressLookupResponse>> LookupAsync(AddressLookupRequest request);
}
