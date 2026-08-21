using System.Text.Json;
using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.ExternalTypes;
using DCWS.MainSite.Api.Domain.Services;
using DCWS.MainSite.Api.Domain.VendorTypes;

namespace DCWS.MainSite.Api.Tests.Services;

public sealed class AddressServiceTests
{
    private sealed class FakeUsGeocoderClient(UsGeocoderResponse? response) : IUsGeocoderClient
    {
        public int CallCount { get; private set; }

        public Task<UsGeocoderResponse?> USAddressLookupAsync(AddressLookupRequest request)
        {
            CallCount++;
            return Task.FromResult(response);
        }
    }

    private static AddressLookupRequest ValidRequest() => new()
    {
        Street = "150 N Capitol Blvd",
        City = "Boise",
        State = "ID",
        ZipCode = "83702"
    };

    [Fact]
    public async Task LookupAsync_ReturnsSuccessfulResponse_WhenMatchFound()
    {
        var geocoderResponse = new UsGeocoderResponse
        {
            Result = new UsGeocoderResult
            {
                AddressMatches =
                [
                    new UsGeocoderAddressMatch
                    {
                        MatchedAddress = "150 N CAPITOL BLVD, BOISE, ID, 83702",
                        Coordinates = new UsGeocoderCoordinates { X = -116.202, Y = 43.615 },
                        Geographies = new Dictionary<string, List<UsGeocoderGeography>>
                        {
                            ["States"] = [new UsGeocoderGeography { Name = "Idaho" }],
                            ["Counties"] = [new UsGeocoderGeography { Name = "Ada County" }],
                            ["119th Congressional Districts"] = [CreateGeography("Congressional District 2", "CD119", "02")],
                            ["2024 State Legislative Districts - Upper"] = [new UsGeocoderGeography { Name = "State Senate District 17", StateLegislativeDistrictUpper = "017" }],
                            ["2024 State Legislative Districts - Lower"] = [new UsGeocoderGeography { Name = "State House District 17", StateLegislativeDistrictLower = "017" }]
                        }
                    }
                ]
            }
        };
        var client = new FakeUsGeocoderClient(geocoderResponse);
        var service = new AddressService(client);

        var result = await service.LookupAsync(ValidRequest());

        Assert.Equal(1, client.CallCount);
        Assert.True(result.WasSuccessful);
        Assert.NotNull(result.Item);
        Assert.Equal("150 N CAPITOL BLVD, BOISE, ID, 83702", result.Item!.MatchedAddress);
        Assert.Equal(43.615, result.Item.Latitude);
        Assert.Equal(-116.202, result.Item.Longitude);
        Assert.Equal("Idaho", result.Item.State);
        Assert.Equal("Ada County", result.Item.County);
        Assert.Equal("02", result.Item.CongressionalDistrict);
        Assert.Equal("017", result.Item.StateSenateDistrict);
        Assert.Equal("017", result.Item.StateHouseDistrict);
    }

    [Fact]
    public async Task LookupAsync_ReturnsUnsuccessfulResponse_WhenNoMatchFound()
    {
        var geocoderResponse = new UsGeocoderResponse
        {
            Result = new UsGeocoderResult
            {
                AddressMatches = []
            }
        };
        var client = new FakeUsGeocoderClient(geocoderResponse);
        var service = new AddressService(client);

        var result = await service.LookupAsync(ValidRequest());

        Assert.Equal(1, client.CallCount);
        Assert.False(result.WasSuccessful);
        Assert.Equal("No matching address was found.", result.Message);
        Assert.Null(result.Item);
    }

    [Fact]
    public async Task LookupAsync_ReturnsNullGeographyValues_WhenGeographiesMissing()
    {
        var geocoderResponse = new UsGeocoderResponse
        {
            Result = new UsGeocoderResult
            {
                AddressMatches =
                [
                    new UsGeocoderAddressMatch
                    {
                        MatchedAddress = "150 N CAPITOL BLVD, BOISE, ID, 83702",
                        Coordinates = new UsGeocoderCoordinates { X = -116.202, Y = 43.615 },
                        Geographies = null
                    }
                ]
            }
        };
        var client = new FakeUsGeocoderClient(geocoderResponse);
        var service = new AddressService(client);

        var result = await service.LookupAsync(ValidRequest());

        Assert.True(result.WasSuccessful);
        Assert.NotNull(result.Item);
        Assert.Null(result.Item!.County);
        Assert.Null(result.Item.State);
        Assert.Null(result.Item.CongressionalDistrict);
        Assert.Null(result.Item.StateHouseDistrict);
        Assert.Null(result.Item.StateSenateDistrict);
    }

    [Fact]
    public async Task LookupAsync_MapsCongressionalDistrict_ForDifferentSessionNumber()
    {
        var geocoderResponse = new UsGeocoderResponse
        {
            Result = new UsGeocoderResult
            {
                AddressMatches =
                [
                    new UsGeocoderAddressMatch
                    {
                        MatchedAddress = "150 N CAPITOL BLVD, BOISE, ID, 83702",
                        Coordinates = new UsGeocoderCoordinates { X = -116.202, Y = 43.615 },
                        Geographies = new Dictionary<string, List<UsGeocoderGeography>>
                        {
                            ["120th Congressional Districts"] = [CreateGeography("Congressional District 2", "CD120", "02")]
                        }
                    }
                ]
            }
        };
        var client = new FakeUsGeocoderClient(geocoderResponse);
        var service = new AddressService(client);

        var result = await service.LookupAsync(ValidRequest());

        Assert.True(result.WasSuccessful);
        Assert.Equal("02", result.Item!.CongressionalDistrict);
    }

    private static UsGeocoderGeography CreateGeography(string name, string congressionalDistrictPropertyName, string congressionalDistrictValue)
    {
        var json = $$"""{"NAME":"{{name}}","{{congressionalDistrictPropertyName}}":"{{congressionalDistrictValue}}"}""";
        return JsonSerializer.Deserialize<UsGeocoderGeography>(json)!;
    }

    [Theory]
    [InlineData(null, "Boise", "ID", "83702", "Street")]
    [InlineData("150 N Capitol Blvd", null, "ID", "83702", "City")]
    [InlineData("150 N Capitol Blvd", "Boise", null, "83702", "State")]
    [InlineData("150 N Capitol Blvd", "Boise", "ID", null, "ZipCode")]
    public async Task LookupAsync_ReturnsValidationIssue_WhenRequiredFieldMissing(
        string? street, string? city, string? state, string? zipCode, string expectedProperty)
    {
        var client = new FakeUsGeocoderClient(response: null);
        var service = new AddressService(client);
        var request = new AddressLookupRequest
        {
            Street = street,
            City = city,
            State = state,
            ZipCode = zipCode
        };

        var result = await service.LookupAsync(request);

        Assert.Equal(0, client.CallCount);
        Assert.False(result.WasSuccessful);
        Assert.Equal("Validation failed.", result.Message);
        Assert.NotNull(result.ValidationIssues);
        Assert.Contains(result.ValidationIssues!, issue => issue.Property == expectedProperty);
    }
}
