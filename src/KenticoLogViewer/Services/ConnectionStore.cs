using System.IO;
using System.Text.Json;
using KenticoLogViewer.Models;

namespace KenticoLogViewer.Services;

public class ConnectionStore : IConnectionStore
{
    private static readonly string StorePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "KenticoLogViewer",
        "connections.json");

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public IReadOnlyList<ConnectionConfig> Load()
    {
        if (!File.Exists(StorePath))
            return [];

        var json = File.ReadAllText(StorePath);
        return JsonSerializer.Deserialize<List<ConnectionConfig>>(json, JsonOptions) ?? [];
    }

    public void Save(IEnumerable<ConnectionConfig> connections)
    {
        var dir = Path.GetDirectoryName(StorePath)!;
        Directory.CreateDirectory(dir);
        var json = JsonSerializer.Serialize(connections, JsonOptions);
        File.WriteAllText(StorePath, json);
    }
}
