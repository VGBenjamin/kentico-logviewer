using KenticoLogViewer.Models;

namespace KenticoLogViewer.Services;

public interface ILogRepository
{
    Task<IReadOnlyList<EventLogEntry>> GetLogsAsync(
        string connectionString,
        string tableName,
        int maxRows,
        LogFilter filter,
        int commandTimeout = 30,
        CancellationToken cancellationToken = default);

    Task<EventLogEntry?> GetByIdAsync(
        string connectionString,
        string tableName,
        int eventId,
        int commandTimeout = 30,
        CancellationToken cancellationToken = default);

    Task<bool> TestConnectionAsync(
        string connectionString,
        string tableName,
        CancellationToken cancellationToken = default);
}
