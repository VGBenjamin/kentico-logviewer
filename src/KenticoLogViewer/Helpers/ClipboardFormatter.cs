using System.Text;
using KenticoLogViewer.Models;

namespace KenticoLogViewer.Helpers;

public static class ClipboardFormatter
{
    private static readonly string[] Headers =
        ["EventID", "EventType", "EventTime", "Source", "EventCode", "EventDescription"];

    public static string Format(IEnumerable<EventLogEntry> entries)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(" | ", Headers));
        foreach (var e in entries)
        {
            sb.AppendLine(
                $"{e.EventID} | {e.EventType} | {e.EventTime:yyyy-MM-dd HH:mm:ss} | {e.Source} | {e.EventCode} | {e.EventDescription}");
        }
        return sb.ToString();
    }
}
