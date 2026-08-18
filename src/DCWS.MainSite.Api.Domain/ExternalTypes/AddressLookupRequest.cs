namespace DCWS.MainSite.Api.Domain.ExternalTypes;

public class AddressLookupRequest
{
    public string? Street { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? ZipCode { get; set; }
}
