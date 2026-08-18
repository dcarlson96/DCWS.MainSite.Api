namespace DCWS.MainSite.Api.Domain.Utilities;

public class ApiListResponse<T> : ApiResponse
{
    public List<T> Items { get; set; } = [];
}
