namespace DCWS.MainSite.Api.Domain.ExternalTypes;

public sealed class TestimonialSubmitRequest
{
    public string? Name { get; set; }

    public string? Organization { get; set; }

    public string? Review { get; set; }

    // Honeypot field. The public form leaves this blank.
    public string? Website { get; set; }
}
