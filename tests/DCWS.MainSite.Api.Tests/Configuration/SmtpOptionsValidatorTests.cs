using DCWS.MainSite.Api.Web.Configuration;

namespace DCWS.MainSite.Api.Tests.Configuration;

public sealed class SmtpOptionsValidatorTests
{
    private readonly SmtpOptionsValidator _validator = new();

    [Fact]
    public void Validate_Succeeds_ForCompleteMicrosoft365Configuration()
    {
        var result = _validator.Validate(null, ValidOptions());

        Assert.True(result.Succeeded);
    }

    [Theory]
    [InlineData(nameof(SmtpOptions.TenantId), "Smtp:TenantId is not configured.")]
    [InlineData(nameof(SmtpOptions.ClientId), "Smtp:ClientId is not configured.")]
    [InlineData(nameof(SmtpOptions.ClientSecret), "Smtp:ClientSecret is not configured.")]
    [InlineData(nameof(SmtpOptions.Username), "Smtp:Username is not configured.")]
    public void Validate_FailsClearly_WhenOAuthSettingIsMissing(
        string propertyName,
        string expectedFailure)
    {
        var options = ValidOptions();
        typeof(SmtpOptions).GetProperty(propertyName)!.SetValue(options, string.Empty);

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(expectedFailure, result.Failures);
    }

    [Fact]
    public void Validate_Fails_WhenStartTlsIsDisabled()
    {
        var options = ValidOptions();
        options.UseStartTls = false;

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(
            "Smtp:UseStartTls must be enabled for Microsoft 365 SMTP.",
            result.Failures);
    }

    private static SmtpOptions ValidOptions() => new()
    {
        Host = "smtp.office365.com",
        Port = 587,
        UseStartTls = true,
        TenantId = "tenant-id",
        ClientId = "client-id",
        ClientSecret = "client-secret",
        Username = "dylan@dcwebsystems.com",
        FromAddress = "dylan@dcwebsystems.com",
        FromName = "DC Web Systems"
    };
}
