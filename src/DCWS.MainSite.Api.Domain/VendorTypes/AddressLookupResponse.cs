namespace DCWS.MainSite.Api.Domain.VendorTypes;

public class AddressLookupResponse
{
    public string? MatchedAddress { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }
}
