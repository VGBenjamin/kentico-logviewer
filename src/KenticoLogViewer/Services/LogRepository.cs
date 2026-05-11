using System.Text.RegularExpressions;
using Dapper;
using Microsoft.Data.SqlClient;
using KenticoLogViewer.Models;

namespace KenticoLogViewer.Services;

public class LogRepository : ILogRepository
{
    private static readonly Regex TableNameRegex =
        new(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

    private const string GridColumns =
        "EventID, EventType, EventTime, Source, EventCode, EventDescription";

    private const string DetailColumns =
        "EventID, EventType, EventTime, Source, EventCode, EventDescription, " +
        "UserID, UserName, IPAddress, EventUrl, EventMachineName, EventUserAgent, EventUrlReferrer";

    public async Task<IReadOnlyList<EventLogEntry>> GetLogsAsync(
        string connectionString,
        string tableName,
        int maxRows,
        LogFilter filter,
        int commandTimeout = 30,
        CancellationToken cancellationToken = default)
    {
        ValidateTableName(tableName);

        var (whereClause, parameters) = BuildWhereClause(filter);
        parameters.Add("MaxRows", maxRows);

        var sql = $"""
            SELECT TOP (@MaxRows) {GridColumns}
            FROM [{tableName}]
            {whereClause}
            ORDER BY EventID DESC
            """;

        await using var conn = new SqlConnection(connectionString);
        var cmd = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken, commandTimeout: commandTimeout);
        var result = await conn.QueryAsync<EventLogEntry>(cmd);
        return result.ToList().AsReadOnly();
    }

    public async Task<EventLogEntry?> GetByIdAsync(
        string connectionString,
        string tableName,
        int eventId,
        int commandTimeout = 30,
        CancellationToken cancellationToken = default)
    {
        ValidateTableName(tableName);

        var sql = $"""
            SELECT {DetailColumns}
            FROM [{tableName}]
            WHERE EventID = @EventID
            """;

        await using var conn = new SqlConnection(connectionString);
        var cmd = new CommandDefinition(sql, new { EventID = eventId }, cancellationToken: cancellationToken, commandTimeout: commandTimeout);
        return await conn.QuerySingleOrDefaultAsync<EventLogEntry>(cmd);
    }

    public async Task<bool> TestConnectionAsync(
        string connectionString,
        string tableName,
        CancellationToken cancellationToken = default)
    {
        ValidateTableName(tableName);

        var sql = $"SELECT TOP 1 EventID FROM [{tableName}]";
        await using var conn = new SqlConnection(connectionString);
        var cmd = new CommandDefinition(sql, cancellationToken: cancellationToken, commandTimeout: 10);
        await conn.QueryAsync<int>(cmd);
        return true;
    }

    private static void ValidateTableName(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName) || !TableNameRegex.IsMatch(tableName))
            throw new ArgumentException($"Invalid table name: '{tableName}'.", nameof(tableName));
    }

    private static (string whereClause, DynamicParameters parameters) BuildWhereClause(LogFilter filter)
    {
        var where = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(filter.EventIdMinFilter)
            && int.TryParse(filter.EventIdMinFilter, out var minId))
        {
            where.Add("EventID >= @EventIdMin");
            parameters.Add("EventIdMin", minId);
        }

        if (!string.IsNullOrWhiteSpace(filter.EventIdMaxFilter)
            && int.TryParse(filter.EventIdMaxFilter, out var maxId))
        {
            where.Add("EventID <= @EventIdMax");
            parameters.Add("EventIdMax", maxId);
        }

        if (filter.EventTypes.Count > 0)
        {
            where.Add("EventType IN @EventTypes");
            parameters.Add("EventTypes", filter.EventTypes);
        }

        if (filter.DateFrom.HasValue)
        {
            where.Add("EventTime >= @DateFrom");
            parameters.Add("DateFrom", filter.DateFrom.Value);
        }

        if (filter.DateTo.HasValue)
        {
            where.Add("EventTime <= @DateTo");
            parameters.Add("DateTo", filter.DateTo.Value);
        }

        AddLikeFilter(where, parameters, filter.SourceFilter, "Source");
        AddLikeFilter(where, parameters, filter.EventCodeFilter, "EventCode");
        AddLikeFilter(where, parameters, filter.EventDescriptionFilter, "EventDescription");
        AddLikeFilter(where, parameters, filter.UserNameFilter, "UserName");
        AddLikeFilter(where, parameters, filter.IpAddressFilter, "IPAddress");
        AddLikeFilter(where, parameters, filter.EventUrlFilter, "EventUrl");
        AddLikeFilter(where, parameters, filter.EventMachineNameFilter, "EventMachineName");
        AddLikeFilter(where, parameters, filter.EventUserAgentFilter, "EventUserAgent");
        AddLikeFilter(where, parameters, filter.EventUrlReferrerFilter, "EventUrlReferrer");

        var whereClause = where.Count > 0
            ? "WHERE " + string.Join(" AND ", where)
            : string.Empty;

        return (whereClause, parameters);
    }

    private static void AddLikeFilter(
        List<string> where,
        DynamicParameters parameters,
        string? value,
        string columnName)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        where.Add($"{columnName} LIKE @{columnName}");
        parameters.Add(columnName, $"%{value}%");
    }
}
