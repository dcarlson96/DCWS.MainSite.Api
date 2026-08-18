namespace DCWS.MainSite.Api.Domain.Utilities;

public class ApiResponse<T> : ApiResponse
{
    public T? Item { get; set; }
}
