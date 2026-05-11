namespace KenticoLogViewer.Models;

public class EventLogEntry
{
    public int EventID { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateTime EventTime { get; set; }
    public string Source { get; set; } = string.Empty;
    public string EventCode { get; set; } = string.Empty;
    public string EventDescription { get; set; } = string.Empty;

    // Hidden columns — loaded only in detail view (GetByIdAsync)
    public int? UserID { get; set; }
    public string? UserName { get; set; }
    public string? IPAddress { get; set; }
    public string? EventUrl { get; set; }
    public string? EventMachineName { get; set; }
    public string? EventUserAgent { get; set; }
    public string? EventUrlReferrer { get; set; }
}
