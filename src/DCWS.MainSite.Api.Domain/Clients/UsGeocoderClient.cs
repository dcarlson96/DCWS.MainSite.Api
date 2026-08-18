using System.Net.Http.Json;
using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.ExternalTypes;

namespace DCWS.MainSite.Api.Domain.Clients;

public sealed class UsGeocoderClient(HttpClient httpClient) : IUsGeocoderClient
{
    public async Task<UsGeocoderResponse?> USAddressLookupAsync(AddressLookupRequest request)
    {
        var query = string.Join('&',
            $"street={Uri.EscapeDataString(request.Street ?? string.Empty)}",
            $"city={Uri.EscapeDataString(request.City ?? string.Empty)}",
            $"state={Uri.EscapeDataString(request.State ?? string.Empty)}",
            $"zip={Uri.EscapeDataString(request.ZipCode ?? string.Empty)}",
            "benchmark=Public_AR_Current",
            "format=json");

        var requestUri = $"geocoder/locations/address?{query}";

        return await httpClient.GetFromJsonAsync<UsGeocoderResponse>(requestUri);
    }
}
