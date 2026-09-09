using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.Models;
using DCWS.MainSite.Api.Web.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Identity.Client;
using Microsoft.Extensions.Options;
using MimeKit;

namespace DCWS.MainSite.Api.Web.Services;

public sealed class SmtpEmailService : IEmailService
{
    private static readonly string[] Scopes = ["https://outlook.office365.com/.default"];
    private readonly IConfidentialClientApplication _confidentialClientApplication;
    private readonly SmtpOptions _settings;

    public SmtpEmailService(IOptions<SmtpOptions> options)
    {
        _settings = options.Value;

        var validationResult = new SmtpOptionsValidator().Validate(Options.DefaultName, _settings);
        if (validationResult.Failed)
        {
            throw new OptionsValidationException(
                Options.DefaultName,
                typeof(SmtpOptions),
                validationResult.Failures);
        }

        _confidentialClientApplication = ConfidentialClientApplicationBuilder
            .Create(_settings.ClientId)
            .WithTenantId(_settings.TenantId)
            .WithClientSecret(_settings.ClientSecret)
            .Build();
    }

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
        email.To.Add(MailboxAddress.Parse(message.Recipient));
        email.Subject = message.Subject;
        email.Body = new BodyBuilder
        {
            HtmlBody = message.HtmlBody,
            TextBody = message.TextBody
        }.ToMessageBody();

        var authenticationResult = await _confidentialClientApplication
            .AcquireTokenForClient(Scopes)
            .ExecuteAsync(cancellationToken);

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.StartTls,
                cancellationToken);
            await client.AuthenticateAsync(
                new SaslMechanismOAuth2(_settings.Username, authenticationResult.AccessToken),
                cancellationToken);

            await client.SendAsync(email, cancellationToken);
        }
        finally
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync(quit: true, CancellationToken.None);
            }
        }
    }
}
