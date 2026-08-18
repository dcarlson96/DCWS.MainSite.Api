namespace DCWS.MainSite.Api.Domain.Utilities;

public abstract class ApiResponse
{
    public bool WasSuccessful { get; set; }

    public string? Message { get; set; }

    public List<ValidationIssue>? ValidationIssues { get; set; }
}
