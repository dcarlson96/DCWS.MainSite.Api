using System.Text.Json.Serialization;

namespace DCWS.MainSite.Api.Domain.Clients;

public sealed class UsGeocoderResponse
{
    [JsonPropertyName("result")]
    public UsGeocoderResult? Result { get; set; }
}

public sealed class UsGeocoderResult
{
    [JsonPropertyName("addressMatches")]
    public List<UsGeocoderAddressMatch> AddressMatches { get; set; } = [];
}

public sealed class UsGeocoderAddressMatch
{
    [JsonPropertyName("matchedAddress")]
    public string? MatchedAddress { get; set; }

    [JsonPropertyName("coordinates")]
    public UsGeocoderCoordinates? Coordinates { get; set; }
}

public sealed class UsGeocoderCoordinates
{
    [JsonPropertyName("x")]
    public double? X { get; set; }

    [JsonPropertyName("y")]
    public double? Y { get; set; }
}
