using System.Globalization;
using System.Net;
using System.Net.Sockets;
using Microsoft.Data.SqlClient;
using SchoolAccount.E2ETests.Common.Configuration;

namespace SchoolAccount.E2ETests.Common.Database;

/// <summary>
/// Runs SQL against one database on a local SQL Server. Refuses any server that isn't on this
/// machine, because the tests that use it change data.
/// </summary>
public sealed class LocalDatabase
{
    // In a data source, a port follows a comma ("localhost,1433") and an instance name a backslash.
    private static readonly char[] _portOrInstanceSeparators = [',', '\\'];

    private readonly string _connectionString;

    private LocalDatabase(DatabaseSettings settings, string databaseName)
    {
        EnsureServerIsLocal(settings.Server);

        _connectionString = new SqlConnectionStringBuilder
        {
            DataSource = settings.Server,
            InitialCatalog = databaseName,
            UserID = settings.UserId,
            Password = settings.Password,
            TrustServerCertificate = true,
        }.ConnectionString;
    }

    public static LocalDatabase CollectPortal(DatabaseSettings settings)
    {
        return new(settings, "COLLECTPortal");
    }

    public static LocalDatabase CollectStateLedger(DatabaseSettings settings)
    {
        return new(settings, "CollectStateLedger");
    }

    public async Task<int> ExecuteNonQueryAsync(string sql, object? parameters = null)
    {
        using var connection = await OpenConnectionAsync();
        using var command = new SqlCommand(sql, connection);
        AddParameters(command, parameters);

        return await command.ExecuteNonQueryAsync();
    }

    /// <summary>Inserts a row and returns the database-generated Id of the new row.</summary>
    public async Task<long> InsertAndReturnIdAsync<T>(T record, string tableName)
        where T : notnull
    {
        using var connection = await OpenConnectionAsync();
        using var command = new SqlCommand(BuildInsertSql<T>(tableName), connection);
        AddParameters(command, record);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt64(result, CultureInfo.InvariantCulture);
    }

    private static string BuildInsertSql<T>(string tableName)
    {
        var columnNames = typeof(T).GetProperties().Select(p => p.Name).ToList();
        var columns = string.Join(", ", columnNames.Select(name => $"[{name}]"));
        var parameterNames = string.Join(", ", columnNames.Select(name => $"@{name}"));
        return $"INSERT INTO [{tableName}] ({columns}) OUTPUT INSERTED.Id VALUES ({parameterNames})";
    }

    public async Task<List<Dictionary<string, object?>>> SelectAsync(
        string sql,
        object? parameters = null
    )
    {
        using var connection = await OpenConnectionAsync();
        using var command = new SqlCommand(sql, connection);
        AddParameters(command, parameters);
        using var reader = await command.ExecuteReaderAsync();

        var rows = new List<Dictionary<string, object?>>();
        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
            }

            rows.Add(row);
        }

        return rows;
    }

    /// <summary>Deletes the rows with the given Ids. Each Id is passed as a SQL parameter.</summary>
    public async Task<int> DeleteByIdsAsync(string tableName, IReadOnlyList<int> ids)
    {
        if (ids.Count == 0)
        {
            throw new ArgumentException("At least one Id is required to delete rows.", nameof(ids));
        }

        var parameterNames = ids.Select((_, index) => $"@Id{index}").ToList();

        using var connection = await OpenConnectionAsync();
        using var command = new SqlCommand(
            $"DELETE FROM [{tableName}] WHERE [Id] IN ({string.Join(", ", parameterNames)})",
            connection
        );

        for (var i = 0; i < ids.Count; i++)
        {
            command.Parameters.AddWithValue(parameterNames[i], ids[i]);
        }

        return await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Stops the suite touching any database except one on this machine. These tests change
    /// data, so running them anywhere else needs this check to be changed deliberately, in code.
    /// </summary>
    private static void EnsureServerIsLocal(string server)
    {
        var host = GetHost(server);
        var isLocal =
            host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
            || IsLoopbackIPv4Address(host);

        if (!isLocal)
        {
            throw new InvalidOperationException(
                $"Database:Server is '{server}'. The database tests only run against localhost "
                    + "or a 127.x.x.x address, because they change data."
            );
        }
    }

    private static bool IsLoopbackIPv4Address(string host)
    {
        return IPAddress.TryParse(host, out var address)
            && address.AddressFamily == AddressFamily.InterNetwork
            && address.GetAddressBytes()[0] == 127;
    }

    /// <summary>
    /// The host part of a SQL Server data source, e.g. "localhost" from "tcp:localhost,1433"
    /// or "localhost\SQLEXPRESS".
    /// </summary>
    private static string GetHost(string server)
    {
        var withoutProtocol = server.Contains(':', StringComparison.Ordinal)
            ? server[(server.IndexOf(':', StringComparison.Ordinal) + 1)..]
            : server;

        return withoutProtocol.Split(_portOrInstanceSeparators)[0].Trim();
    }

    private async Task<SqlConnection> OpenConnectionAsync()
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    private static void AddParameters(SqlCommand command, object? parameters)
    {
        if (parameters is null)
        {
            return;
        }

        foreach (var property in parameters.GetType().GetProperties())
        {
            command.Parameters.AddWithValue(
                $"@{property.Name}",
                property.GetValue(parameters) ?? DBNull.Value
            );
        }
    }
}
