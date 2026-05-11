namespace KenticoLogViewer.Models;

public class LogFilter
{
    public string? EventIdMinFilter { get; set; }
    public string? EventIdMaxFilter { get; set; }

    public List<string> EventTypes { get; set; } = [];

    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }

    public string? SourceFilter { get; set; }
    public string? EventCodeFilter { get; set; }
    public string? EventDescriptionFilter { get; set; }

    // Advanced — hidden columns
    public string? UserIdFilter { get; set; }
    public string? UserNameFilter { get; set; }
    public string? IpAddressFilter { get; set; }
    public string? EventUrlFilter { get; set; }
    public string? EventMachineNameFilter { get; set; }
    public string? EventUserAgentFilter { get; set; }
    public string? EventUrlReferrerFilter { get; set; }
}
