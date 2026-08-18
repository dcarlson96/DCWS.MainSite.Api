namespace DCWS.MainSite.Api.Domain.ExternalTypes;

public sealed record StatusResponse(
    string Status,
    string Database,
    int? Id,
    string? Message,
    DateTimeOffset? CreatedDateUtc);
