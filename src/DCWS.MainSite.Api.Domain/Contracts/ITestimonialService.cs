using DCWS.MainSite.Api.Domain.ExternalTypes;
using DCWS.MainSite.Api.Domain.Utilities;

namespace DCWS.MainSite.Api.Domain.Contracts;

public interface ITestimonialService
{
    Task<ApiResponse<TestimonialSubmitResponse>> SubmitAsync(
        TestimonialSubmitRequest request,
        CancellationToken cancellationToken = default);
}
