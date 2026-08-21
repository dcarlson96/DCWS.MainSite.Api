using DCWS.MainSite.Api.Domain.ExternalTypes;
using DCWS.MainSite.Api.Domain.VendorTypes;

namespace DCWS.MainSite.Api.Domain.Contracts;

public interface IUsGeocoderClient
{
    Task<UsGeocoderResponse?> USAddressLookupAsync(AddressLookupRequest request);
}
