using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DCWS.MainSite.Api.Domain.Services;

public sealed class StatusService(IConfiguration configuration) : IStatusService
{
    private const string SelectLatestStatusSql = """
        SELECT TOP 1
            Id,
            Message,
            CreatedDateUtc
        FROM dbo.StatusTest
        ORDER BY Id DESC;
        """;

    public async Task<StatusResponse> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("The 'DefaultConnection' connection string is not configured.");

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(SelectLatestStatusSql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(reader.GetOrdinal("Id"));
            var message = reader.GetString(reader.GetOrdinal("Message"));
            var createdDateUtc = reader.GetDateTime(reader.GetOrdinal("CreatedDateUtc"));

            return new StatusResponse(
                Status: "ok",
                Database: "connected",
                Id: id,
                Message: message,
                CreatedDateUtc: new DateTimeOffset(createdDateUtc, TimeSpan.Zero));
        }

        return new StatusResponse(
            Status: "ok",
            Database: "connected",
            Id: null,
            Message: "No status record found.",
            CreatedDateUtc: null);
    }
}
