using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.ExternalTypes;
using DCWS.MainSite.Api.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DCWS.MainSite.Api.Web.Controllers;

[ApiController]
[Route("api/testimonials")]
public sealed class TestimonialsController(ITestimonialService testimonialService) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("TestimonialSubmission")]
    public Task<ApiResponse<TestimonialSubmitResponse>> Submit(
        [FromBody] TestimonialSubmitRequest request,
        CancellationToken cancellationToken)
    {
        return testimonialService.SubmitAsync(request, cancellationToken);
    }
}
