namespace DCWS.MainSite.Api.Domain.Models;

public sealed record EmailMessage(
    string Recipient,
    string Subject,
    string HtmlBody,
    string TextBody);
