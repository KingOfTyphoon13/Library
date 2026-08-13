using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Library.DAL;

public sealed class DbInitializer
{
    private readonly string _connectionString;
    private readonly IHostEnvironment _env;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(IConfiguration config, IHostEnvironment env, ILogger<DbInitializer> logger)
    {
        _connectionString = config.GetConnectionString("DBConnection")
            ?? throw new InvalidOperationException("Missing 'Default' connection string.");
        _env = env;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        var builder = new SqlConnectionStringBuilder(_connectionString);
        var targetDb = builder.InitialCatalog;
        builder.InitialCatalog = DALConstantsLocator.InitialCatalog;

        await using var conn = new SqlConnection(builder.ConnectionString);
        await conn.OpenAsync(ct);

        if (!await DatabaseExistsAsync(conn, targetDb, ct))
        {
            _logger.LogInformation("Database {Db} not found, creating ({Env}).", targetDb, _env.EnvironmentName);

            await using (var createCmd = new SqlCommand($"CREATE DATABASE [{targetDb}]", conn))
                await createCmd.ExecuteNonQueryAsync(ct);

            await RunScriptAsync(DALConstantsLocator.ScriptsFiles.InitializeDb, targetDb, ct);
        }

        if (_env.IsDevelopment())
        {
            await RunScriptAsync(DALConstantsLocator.ScriptsFiles.SeedDb, targetDb, ct);
        }
    }

    private static async Task<bool> DatabaseExistsAsync(SqlConnection conn, string dbName, CancellationToken ct)
    {
        await using var cmd = new SqlCommand("SELECT 1 FROM sys.databases WHERE name = @name", conn);
        cmd.Parameters.AddWithValue("@name", dbName);
        var result = await cmd.ExecuteScalarAsync(ct);
        return result is not null;
    }

    private async Task RunScriptAsync(string resourceName, string dbName, CancellationToken ct)
    {
        var asm = Assembly.GetExecutingAssembly();
        await using var stream = asm.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded script not found: {resourceName}");
        using var reader = new StreamReader(stream);
        var script = await reader.ReadToEndAsync(ct);

        var targetBuilder = new SqlConnectionStringBuilder(_connectionString) { InitialCatalog = dbName };
        await using var conn = new SqlConnection(targetBuilder.ConnectionString);
        await conn.OpenAsync(ct);

        foreach (var batch in Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
                                    .Where(b => !string.IsNullOrWhiteSpace(b)))
        {
            await using var cmd = new SqlCommand(batch, conn) { CommandTimeout = 120 };
            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}
