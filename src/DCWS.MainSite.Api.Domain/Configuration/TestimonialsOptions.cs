namespace DCWS.MainSite.Api.Domain.Configuration;

public sealed class TestimonialsOptions
{
    public const string SectionName = "Testimonials";

    public string NotificationEmail { get; set; } = string.Empty;
}
