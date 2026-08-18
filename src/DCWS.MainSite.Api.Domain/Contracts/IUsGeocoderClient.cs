using DCWS.MainSite.Api.Domain.Clients;
using DCWS.MainSite.Api.Domain.ExternalTypes;

namespace DCWS.MainSite.Api.Domain.Contracts;

public interface IUsGeocoderClient
{
    Task<UsGeocoderResponse?> USAddressLookupAsync(AddressLookupRequest request);
}
