using Microsoft.Extensions.Options;

namespace DCWS.MainSite.Api.Web.Configuration;

public sealed class SmtpOptionsValidator : IValidateOptions<SmtpOptions>
{
    public ValidateOptionsResult Validate(string? name, SmtpOptions options)
    {
        var failures = new List<string>();

        AddRequiredFailure(failures, options.Host, "Smtp:Host");
        AddRequiredFailure(failures, options.TenantId, "Smtp:TenantId");
        AddRequiredFailure(failures, options.ClientId, "Smtp:ClientId");
        AddRequiredFailure(failures, options.ClientSecret, "Smtp:ClientSecret");
        AddRequiredFailure(failures, options.Username, "Smtp:Username");
        AddRequiredFailure(failures, options.FromAddress, "Smtp:FromAddress");
        AddRequiredFailure(failures, options.FromName, "Smtp:FromName");

        if (options.Port is < 1 or > 65535)
        {
            failures.Add("Smtp:Port must be between 1 and 65535.");
        }

        if (!options.UseStartTls)
        {
            failures.Add("Smtp:UseStartTls must be enabled for Microsoft 365 SMTP.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }

    private static void AddRequiredFailure(
        ICollection<string> failures,
        string value,
        string settingName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            failures.Add($"{settingName} is not configured.");
        }
    }
}
