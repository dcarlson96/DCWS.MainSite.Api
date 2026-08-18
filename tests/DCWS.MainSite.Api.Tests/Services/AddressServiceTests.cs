using DCWS.MainSite.Api.Domain.Clients;
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
                        Coordinates = new UsGeocoderCoordinates { X = -116.202, Y = 43.615 }
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
