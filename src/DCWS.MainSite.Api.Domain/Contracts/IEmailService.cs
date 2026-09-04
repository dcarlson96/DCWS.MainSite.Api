using DCWS.MainSite.Api.Domain.Models;

namespace DCWS.MainSite.Api.Domain.Contracts;

public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
