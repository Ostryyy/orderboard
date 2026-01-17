using Microsoft.Data.SqlClient;

namespace OrderBoard.Api.IntegrationTests.Infrastructure;

public sealed class TestDatabase
{
    public string DatabaseName { get; } = $"OrderBoard_Test_{Guid.NewGuid():N}";
    public string ConnectionString { get; }

    private readonly string _masterConnectionString;

    public TestDatabase()
    {
        var password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD") ?? "Password123!";
        _masterConnectionString =
            $"Server=localhost,1433;Database=master;User Id=sa;Password={password};TrustServerCertificate=True;Encrypt=False";

        ConnectionString =
            $"Server=localhost,1433;Database={DatabaseName};User Id=sa;Password={password};TrustServerCertificate=True;Encrypt=False";
    }

    public async Task CreateAsync()
    {
        await using var conn = new SqlConnection(_masterConnectionString);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = $"IF DB_ID('{DatabaseName}') IS NULL CREATE DATABASE [{DatabaseName}];";
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DropAsync()
    {
        await using var conn = new SqlConnection(_masterConnectionString);
        await conn.OpenAsync();

        var cmd = conn.CreateCommand();
        cmd.CommandText = $@"
IF DB_ID('{DatabaseName}') IS NOT NULL
BEGIN
    ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [{DatabaseName}];
END";
        await cmd.ExecuteNonQueryAsync();
    }
}
