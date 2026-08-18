using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.ExternalTypes;
using DCWS.MainSite.Api.Domain.Utilities;
using DCWS.MainSite.Api.Domain.VendorTypes;
using DCWS.MainSite.Api.Web.Controllers;

namespace DCWS.MainSite.Api.Tests.Controllers;

public sealed class AddressControllerTests
{
    private sealed class FakeAddressService(ApiResponse<AddressLookupResponse> response) : IAddressService
    {
        public AddressLookupRequest? ReceivedRequest { get; private set; }

        public Task<ApiResponse<AddressLookupResponse>> LookupAsync(AddressLookupRequest request)
        {
            ReceivedRequest = request;
            return Task.FromResult(response);
        }
    }

    [Fact]
    public async Task Lookup_DelegatesDirectlyToAddressService()
    {
        var expectedResponse = new ApiResponse<AddressLookupResponse>
        {
            WasSuccessful = true,
            Item = new AddressLookupResponse { MatchedAddress = "150 N CAPITOL BLVD, BOISE, ID, 83702" }
        };
        var service = new FakeAddressService(expectedResponse);
        var controller = new AddressController(service);
        var request = new AddressLookupRequest
        {
            Street = "150 N Capitol Blvd",
            City = "Boise",
            State = "ID",
            ZipCode = "83702"
        };

        var result = await controller.Lookup(request);

        Assert.Same(expectedResponse, result);
        Assert.Same(request, service.ReceivedRequest);
    }
}
