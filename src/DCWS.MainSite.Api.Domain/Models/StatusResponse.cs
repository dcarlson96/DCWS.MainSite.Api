namespace DCWS.MainSite.Api.Domain.Models;

public sealed record StatusResponse(
    string Status,
    string Database,
    int? Id,
    string? Message,
    DateTimeOffset? CreatedDateUtc);
