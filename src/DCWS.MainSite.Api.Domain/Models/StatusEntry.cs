namespace DCWS.MainSite.Api.Domain.Models;

public sealed class StatusEntry
{
    public int Id { get; set; }

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedDateUtc { get; set; }
}
