namespace DCWS.MainSite.Api.Domain.Models;

public sealed record StatusResponse(
    string Message,
    string Status,
    DateTimeOffset TimestampUtc);
