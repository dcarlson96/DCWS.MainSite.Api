using DCWS.MainSite.Api.Domain.Configuration;
using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.ExternalTypes;
using DCWS.MainSite.Api.Domain.Models;
using DCWS.MainSite.Api.Domain.Services;
using DCWS.MainSite.Api.Domain.Utilities;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace DCWS.MainSite.Api.Tests.Services;

public sealed class TestimonialServiceTests
{
    private static readonly DateTimeOffset SubmittedUtc =
        new(2026, 9, 4, 18, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task SubmitAsync_SendsConfiguredEmail_ForValidSubmission()
    {
        var emailService = new FakeEmailService();
        var service = CreateService(emailService);

        var result = await service.SubmitAsync(new TestimonialSubmitRequest
        {
            Name = "  Jane Doe  ",
            Organization = "  Acme Consulting  ",
            Review = "  Dylan was fantastic to work with.  "
        });

        Assert.True(result.WasSuccessful);
        Assert.NotNull(result.Item);
        Assert.True(result.Item.SubmittedForReview);
        var message = Assert.Single(emailService.Messages);
        Assert.Equal("dylan@dcwebsystems.com", message.Recipient);
        Assert.Equal("New DC Web Systems Testimonial Submission", message.Subject);
        Assert.Contains("Jane Doe", message.HtmlBody);
        Assert.Contains("Acme Consulting", message.HtmlBody);
        Assert.Contains("Dylan was fantastic to work with.", message.HtmlBody);
        Assert.Contains("Organization:\r\nAcme Consulting", message.TextBody);
        Assert.Contains("September 4, 2026 at 6:00 PM UTC", message.HtmlBody);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SubmitAsync_RejectsMissingOrWhitespaceOrganization(string? organization)
    {
        var emailService = new FakeEmailService();
        var service = CreateService(emailService);
        var request = ValidRequest();
        request.Organization = organization;

        var result = await service.SubmitAsync(request);

        AssertValidationFailure(result, "Organization", "Organization is required.");
        Assert.Empty(emailService.Messages);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SubmitAsync_RejectsMissingOrWhitespaceName(string? name)
    {
        var emailService = new FakeEmailService();
        var service = CreateService(emailService);

        var result = await service.SubmitAsync(WithName(ValidRequest(), name));

        AssertValidationFailure(result, "Name", "Name is required.");
        Assert.Empty(emailService.Messages);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SubmitAsync_RejectsMissingOrWhitespaceReview(string? review)
    {
        var emailService = new FakeEmailService();
        var service = CreateService(emailService);
        var request = ValidRequest();
        request.Review = review;

        var result = await service.SubmitAsync(request);

        AssertValidationFailure(result, "Review", "Review is required.");
        Assert.Empty(emailService.Messages);
    }

    [Fact]
    public async Task SubmitAsync_RejectsNameOverMaximumLength()
    {
        var service = CreateService(new FakeEmailService());
        var request = ValidRequest();
        request.Name = new string('n', TestimonialService.NameMaximumLength + 1);

        var result = await service.SubmitAsync(request);

        AssertValidationFailure(
            result,
            "Name",
            $"Name cannot exceed {TestimonialService.NameMaximumLength} characters.");
    }

    [Fact]
    public async Task SubmitAsync_RejectsReviewOverMaximumLength()
    {
        var service = CreateService(new FakeEmailService());
        var request = ValidRequest();
        request.Review = new string('r', TestimonialService.ReviewMaximumLength + 1);

        var result = await service.SubmitAsync(request);

        AssertValidationFailure(
            result,
            "Review",
            $"Review cannot exceed {TestimonialService.ReviewMaximumLength} characters.");
    }

    [Fact]
    public async Task SubmitAsync_RejectsOrganizationOverMaximumLength()
    {
        var service = CreateService(new FakeEmailService());
        var request = ValidRequest();
        request.Organization = new string('o', TestimonialService.OrganizationMaximumLength + 1);

        var result = await service.SubmitAsync(request);

        AssertValidationFailure(
            result,
            "Organization",
            $"Organization cannot exceed {TestimonialService.OrganizationMaximumLength} characters.");
    }

    [Fact]
    public async Task SubmitAsync_ReturnsFriendlyFailure_WhenEmailServiceFails()
    {
        var service = CreateService(new FakeEmailService(exception: new InvalidOperationException("SMTP unavailable")));

        var result = await service.SubmitAsync(ValidRequest());

        Assert.False(result.WasSuccessful);
        Assert.Null(result.Item);
        Assert.Equal("We couldn't submit your testimonial right now. Please try again later.", result.Message);
        Assert.DoesNotContain("SMTP", result.Message);
    }

    [Fact]
    public async Task SubmitAsync_HtmlEncodesVisitorContent()
    {
        var emailService = new FakeEmailService();
        var service = CreateService(emailService);

        await service.SubmitAsync(new TestimonialSubmitRequest
        {
            Name = "<strong>Jane</strong>",
            Organization = "<em>Acme</em>",
            Review = "Great work <script>alert('x')</script>"
        });

        var message = Assert.Single(emailService.Messages);
        Assert.DoesNotContain("<script>", message.HtmlBody);
        Assert.Contains("&lt;script&gt;", message.HtmlBody);
        Assert.Contains("&lt;strong&gt;Jane&lt;/strong&gt;", message.HtmlBody);
        Assert.Contains("&lt;em&gt;Acme&lt;/em&gt;", message.HtmlBody);
    }

    private static TestimonialService CreateService(IEmailService emailService)
    {
        return new TestimonialService(
            emailService,
            Options.Create(new TestimonialsOptions
            {
                NotificationEmail = "dylan@dcwebsystems.com"
            }),
            new FixedTimeProvider(SubmittedUtc),
            NullLogger<TestimonialService>.Instance);
    }

    private static TestimonialSubmitRequest ValidRequest() => new()
    {
        Name = "Jane Doe",
        Organization = "Acme Consulting",
        Review = "Dylan was fantastic to work with."
    };

    private static TestimonialSubmitRequest WithName(
        TestimonialSubmitRequest request,
        string? name)
    {
        request.Name = name;
        return request;
    }

    private static void AssertValidationFailure(
        ApiResponse<TestimonialSubmitResponse> result,
        string property,
        string issue)
    {
        Assert.False(result.WasSuccessful);
        Assert.Equal("Validation failed.", result.Message);
        var validationIssue = Assert.Single(result.ValidationIssues!);
        Assert.Equal(property, validationIssue.Property);
        Assert.Equal(issue, validationIssue.Issue);
    }

    private sealed class FakeEmailService(Exception? exception = null) : IEmailService
    {
        public List<EmailMessage> Messages { get; } = [];

        public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            if (exception is not null)
            {
                throw exception;
            }

            Messages.Add(message);
            return Task.CompletedTask;
        }
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
