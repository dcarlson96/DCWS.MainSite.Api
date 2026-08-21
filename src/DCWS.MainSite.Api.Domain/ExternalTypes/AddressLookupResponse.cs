namespace DCWS.MainSite.Api.Domain.ExternalTypes;

public class AddressLookupResponse
{
    public string? MatchedAddress { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? County { get; set; }

    public string? State { get; set; }

    public string? CongressionalDistrict { get; set; }

    public string? StateHouseDistrict { get; set; }

    public string? StateSenateDistrict { get; set; }
}
