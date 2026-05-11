using KenticoLogViewer.Models;

namespace KenticoLogViewer.Services;

public interface IConnectionStore
{
    IReadOnlyList<ConnectionConfig> Load();
    void Save(IEnumerable<ConnectionConfig> connections);
}
