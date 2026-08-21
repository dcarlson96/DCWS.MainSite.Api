using System.Text.Json;
using System.Text.Json.Serialization;

namespace DCWS.MainSite.Api.Domain.VendorTypes;

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

    [JsonPropertyName("geographies")]
    public Dictionary<string, List<UsGeocoderGeography>>? Geographies { get; set; }
}

public sealed class UsGeocoderCoordinates
{
    [JsonPropertyName("x")]
    public double? X { get; set; }

    [JsonPropertyName("y")]
    public double? Y { get; set; }
}

public sealed class UsGeocoderGeography
{
    [JsonPropertyName("NAME")]
    public string? Name { get; set; }

    [JsonPropertyName("SLDU")]
    public string? StateLegislativeDistrictUpper { get; set; }

    [JsonPropertyName("SLDL")]
    public string? StateLegislativeDistrictLower { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }

    public string? GetCongressionalDistrict()
    {
        if (ExtensionData is null)
        {
            return null;
        }

        foreach (var (key, value) in ExtensionData)
        {
            if (key.StartsWith("CD", StringComparison.OrdinalIgnoreCase) &&
                key.Length > 2 &&
                key[2..].All(char.IsDigit) &&
                value.ValueKind == JsonValueKind.String)
            {
                return value.GetString();
            }
        }

        return null;
    }
}

