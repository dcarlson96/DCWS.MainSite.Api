using System.Globalization;
using System.Net;
using DCWS.MainSite.Api.Domain.Configuration;
using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.ExternalTypes;
using DCWS.MainSite.Api.Domain.Models;
using DCWS.MainSite.Api.Domain.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DCWS.MainSite.Api.Domain.Services;

public sealed class TestimonialService(
    IEmailService emailService,
    IOptions<TestimonialsOptions> options,
    TimeProvider timeProvider,
    ILogger<TestimonialService> logger) : ITestimonialService
{
    public const int NameMaximumLength = 100;
    public const int OrganizationMaximumLength = 100;
    public const int ReviewMaximumLength = 2000;

    public async Task<ApiResponse<TestimonialSubmitResponse>> SubmitAsync(
        TestimonialSubmitRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationIssues = Validate(request);

        if (validationIssues.Count > 0)
        {
            return Failure("Validation failed.", validationIssues);
        }

        // Silently accept honeypot submissions without sending mail so bots do not
        // learn that their submission was identified.
        if (!string.IsNullOrWhiteSpace(request.Website))
        {
            return Success();
        }

        var notificationEmail = options.Value.NotificationEmail;

        if (string.IsNullOrWhiteSpace(notificationEmail))
        {
            logger.LogError("Testimonials notification email is not configured.");
            return Failure("Testimonial submissions are temporarily unavailable.");
        }

        var name = request.Name!.Trim();
        var organization = request.Organization!.Trim();
        var review = request.Review!.Trim();
        var submittedUtc = timeProvider.GetUtcNow();
        var message = CreateMessage(notificationEmail, name, organization, review, submittedUtc);

        try
        {
            await emailService.SendAsync(message, cancellationToken);
            return Success();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to send a testimonial submission notification.");
            return Failure("We couldn't submit your testimonial right now. Please try again later.");
        }
    }

    private static EmailMessage CreateMessage(
        string recipient,
        string name,
        string organization,
        string review,
        DateTimeOffset submittedUtc)
    {
        var encodedName = WebUtility.HtmlEncode(name);
        var encodedOrganization = WebUtility.HtmlEncode(organization);
        var encodedReview = WebUtility.HtmlEncode(review)
            .Replace("\r\n", "<br />", StringComparison.Ordinal)
            .Replace("\n", "<br />", StringComparison.Ordinal);
        var submittedDisplay = submittedUtc.ToString(
            "MMMM d, yyyy 'at' h:mm tt 'UTC'",
            CultureInfo.InvariantCulture);

        return new EmailMessage(
            recipient,
            "New DC Web Systems Testimonial Submission",
            $"""
             <p>A new testimonial has been submitted through dcwebsystems.com.</p>
             <p><strong>Name:</strong><br />{encodedName}</p>
             <p><strong>Organization:</strong><br />{encodedOrganization}</p>
             <p><strong>Review:</strong><br />{encodedReview}</p>
             <p><strong>Submitted:</strong><br />{submittedDisplay}</p>
             """,
            $"""
             A new testimonial has been submitted through dcwebsystems.com.

             Name:
             {name}

             Organization:
             {organization}

             Review:
             {review}

             Submitted:
             {submittedDisplay}
             """);
    }

    private static List<ValidationIssue> Validate(TestimonialSubmitRequest request)
    {
        var issues = new List<ValidationIssue>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            issues.Add(new ValidationIssue
            {
                Property = nameof(request.Name),
                Issue = "Name is required."
            });
        }
        else if (request.Name.Length > NameMaximumLength)
        {
            issues.Add(new ValidationIssue
            {
                Property = nameof(request.Name),
                Issue = $"Name cannot exceed {NameMaximumLength} characters."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Organization))
        {
            issues.Add(new ValidationIssue
            {
                Property = nameof(request.Organization),
                Issue = "Organization is required."
            });
        }
        else if (request.Organization.Length > OrganizationMaximumLength)
        {
            issues.Add(new ValidationIssue
            {
                Property = nameof(request.Organization),
                Issue = $"Organization cannot exceed {OrganizationMaximumLength} characters."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Review))
        {
            issues.Add(new ValidationIssue
            {
                Property = nameof(request.Review),
                Issue = "Review is required."
            });
        }
        else if (request.Review.Length > ReviewMaximumLength)
        {
            issues.Add(new ValidationIssue
            {
                Property = nameof(request.Review),
                Issue = $"Review cannot exceed {ReviewMaximumLength} characters."
            });
        }

        return issues;
    }

    private static ApiResponse<TestimonialSubmitResponse> Success() => new()
    {
        WasSuccessful = true,
        Item = new TestimonialSubmitResponse(SubmittedForReview: true)
    };

    private static ApiResponse<TestimonialSubmitResponse> Failure(
        string message,
        List<ValidationIssue>? validationIssues = null) => new()
    {
        WasSuccessful = false,
        Message = message,
        ValidationIssues = validationIssues
    };
}
