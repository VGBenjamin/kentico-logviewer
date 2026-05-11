using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KenticoLogViewer.Models;
using KenticoLogViewer.Services;

namespace KenticoLogViewer.ViewModels;

public record LogField(string Label, string? Value);

public partial class LogDetailViewModel : ObservableObject
{
    private readonly ILogRepository _repository;

    [ObservableProperty] private EventLogEntry? _entry;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = string.Empty;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    partial void OnErrorMessageChanged(string value) => OnPropertyChanged(nameof(HasError));

    public ObservableCollection<LogField> Fields { get; } = [];

    public LogDetailViewModel(ILogRepository repository)
    {
        _repository = repository;
    }

    public async Task LoadAsync(string connectionString, string tableName, int eventId, int commandTimeout = 30)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        Fields.Clear();
        try
        {
            Entry = await _repository.GetByIdAsync(connectionString, tableName, eventId, commandTimeout);
            if (Entry is not null)
                PopulateFields(Entry);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void PopulateFields(EventLogEntry e)
    {
        Fields.Add(new("EventID", e.EventID.ToString()));
        Fields.Add(new("EventType", e.EventType));
        Fields.Add(new("EventTime", e.EventTime.ToString("yyyy-MM-dd HH:mm:ss")));
        Fields.Add(new("Source", e.Source));
        Fields.Add(new("EventCode", e.EventCode));
        Fields.Add(new("EventDescription", e.EventDescription));
        Fields.Add(new("UserID", e.UserID?.ToString()));
        Fields.Add(new("UserName", e.UserName));
        Fields.Add(new("IPAddress", e.IPAddress));
        Fields.Add(new("EventUrl", e.EventUrl));
        Fields.Add(new("EventMachineName", e.EventMachineName));
        Fields.Add(new("EventUserAgent", e.EventUserAgent));
        Fields.Add(new("EventUrlReferrer", e.EventUrlReferrer));
    }

    [RelayCommand]
    private static void CopyField(string? value)
    {
        if (value is not null)
            System.Windows.Clipboard.SetText(value);
    }
}
