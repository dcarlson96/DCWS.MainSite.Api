using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.ExternalTypes;
using DCWS.MainSite.Api.Domain.Utilities;
using DCWS.MainSite.Api.Domain.VendorTypes;

namespace DCWS.MainSite.Api.Domain.Services;

public sealed class AddressService(IUsGeocoderClient usGeocoderClient) : IAddressService
{
    public async Task<ApiResponse<AddressLookupResponse>> LookupAsync(AddressLookupRequest request)
    {
        var validationIssues = Validate(request);

        if (validationIssues.Count > 0)
        {
            return new ApiResponse<AddressLookupResponse>
            {
                WasSuccessful = false,
                Message = "Validation failed.",
                ValidationIssues = validationIssues
            };
        }

        var geocoderResponse = await usGeocoderClient.USAddressLookupAsync(request);

        var match = geocoderResponse?.Result?.AddressMatches.FirstOrDefault();

        if (match is null)
        {
            return new ApiResponse<AddressLookupResponse>
            {
                WasSuccessful = false,
                Message = "No matching address was found."
            };
        }

        var geographies = match.Geographies;

        return new ApiResponse<AddressLookupResponse>
        {
            WasSuccessful = true,
            Item = new AddressLookupResponse
            {
                MatchedAddress = match.MatchedAddress,
                Latitude = match.Coordinates?.Y,
                Longitude = match.Coordinates?.X,
                County = GetGeography(geographies, key => key.Equals("Counties", StringComparison.OrdinalIgnoreCase))?.Name,
                State = GetGeography(geographies, key => key.Equals("States", StringComparison.OrdinalIgnoreCase))?.Name,
                CongressionalDistrict = GetGeography(geographies, key => key.EndsWith("Congressional Districts", StringComparison.OrdinalIgnoreCase))?.GetCongressionalDistrict(),
                StateHouseDistrict = GetGeography(geographies, key => key.Contains("State Legislative Districts - Lower", StringComparison.OrdinalIgnoreCase))?.StateLegislativeDistrictLower,
                StateSenateDistrict = GetGeography(geographies, key => key.Contains("State Legislative Districts - Upper", StringComparison.OrdinalIgnoreCase))?.StateLegislativeDistrictUpper
            }
        };
    }

    private static UsGeocoderGeography? GetGeography(
        Dictionary<string, List<UsGeocoderGeography>>? geographies,
        Func<string, bool> keyPredicate)
    {
        if (geographies is null)
        {
            return null;
        }

        foreach (var (key, value) in geographies)
        {
            if (keyPredicate(key))
            {
                return value.FirstOrDefault();
            }
        }

        return null;
    }

    private static List<ValidationIssue> Validate(AddressLookupRequest request)
    {
        var issues = new List<ValidationIssue>();

        if (string.IsNullOrWhiteSpace(request.Street))
        {
            issues.Add(new ValidationIssue
            {
                Property = nameof(request.Street),
                Issue = "Street is required."
            });
        }

        if (string.IsNullOrWhiteSpace(request.City))
        {
            issues.Add(new ValidationIssue
            {
                Property = nameof(request.City),
                Issue = "City is required."
            });
        }

        if (string.IsNullOrWhiteSpace(request.State))
        {
            issues.Add(new ValidationIssue
            {
                Property = nameof(request.State),
                Issue = "State is required."
            });
        }

        if (string.IsNullOrWhiteSpace(request.ZipCode))
        {
            issues.Add(new ValidationIssue
            {
                Property = nameof(request.ZipCode),
                Issue = "ZipCode is required."
            });
        }

        return issues;
    }
}
