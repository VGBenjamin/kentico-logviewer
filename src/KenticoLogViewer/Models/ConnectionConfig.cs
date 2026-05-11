namespace KenticoLogViewer.Models;

public class ConnectionConfig
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string EncryptedConnectionString { get; set; } = string.Empty;
    public string TableName { get; set; } = "CMS_EventLog";
    public int MaxRows { get; set; } = 500;
    public int CommandTimeout { get; set; } = 30;
}
