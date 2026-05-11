using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KenticoLogViewer.Helpers;
using KenticoLogViewer.Models;
using KenticoLogViewer.Services;

namespace KenticoLogViewer.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IConnectionStore _connectionStore;
    private readonly ILogRepository _repository;
    private readonly DispatcherTimer _refreshTimer;

    private System.Threading.CancellationTokenSource? _loadCts;

    public ObservableCollection<ConnectionConfig> Connections { get; } = [];
    public ObservableCollection<EventLogEntry> Logs { get; } = [];
    public ObservableCollection<EventLogEntry> SelectedLogs { get; } = [];

    public FilterPanelViewModel Filter { get; } = new();

    [ObservableProperty] private ConnectionConfig? _selectedConnection;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private string _lastRefresh = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _autoRefreshEnabled;
    [ObservableProperty] private int _autoRefreshInterval = 30;
    [ObservableProperty] private bool _showAdvancedFilters;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    partial void OnErrorMessageChanged(string value) => OnPropertyChanged(nameof(HasError));

    public int[] RefreshIntervals { get; } = [5, 10, 30, 60];

    public MainViewModel(IConnectionStore connectionStore, ILogRepository repository)
    {
        _connectionStore = connectionStore;
        _repository = repository;

        _refreshTimer = new DispatcherTimer();
        _refreshTimer.Tick += async (_, _) => await LoadLogsAsync();

        Filter.FilterChanged += async (_, _) => await LoadLogsAsync();

        LoadConnections();
    }

    private void LoadConnections()
    {
        Connections.Clear();
        foreach (var c in _connectionStore.Load())
            Connections.Add(c);

        SelectedConnection = Connections.FirstOrDefault();
    }

    partial void OnSelectedConnectionChanged(ConnectionConfig? value)
    {
        if (value is not null)
            _ = LoadLogsAsync();
    }

    partial void OnAutoRefreshEnabledChanged(bool value)
    {
        if (value)
        {
            _refreshTimer.Interval = TimeSpan.FromSeconds(AutoRefreshInterval);
            _refreshTimer.Start();
        }
        else
        {
            _refreshTimer.Stop();
        }
    }

    partial void OnAutoRefreshIntervalChanged(int value)
    {
        _refreshTimer.Interval = TimeSpan.FromSeconds(value);
    }

    [RelayCommand]
    private async Task Refresh() => await LoadLogsAsync();

    [RelayCommand]
    private void ResetFilters() => Filter.Reset();

    [RelayCommand]
    private void ToggleAdvancedFilters() => ShowAdvancedFilters = !ShowAdvancedFilters;

    [RelayCommand]
    private void CopySelected()
    {
        if (SelectedLogs.Count == 0) return;
        var text = ClipboardFormatter.Format(SelectedLogs);
        Clipboard.SetText(text);
    }

    [RelayCommand]
    private void ReloadConnections() => LoadConnections();

    private async Task LoadLogsAsync()
    {
        if (SelectedConnection is null) return;

        _loadCts?.Cancel();
        _loadCts = new System.Threading.CancellationTokenSource();
        var token = _loadCts.Token;

        IsLoading = true;
        StatusMessage = "Loading…";

        string connectionString;
        try
        {
            connectionString = DpapiHelper.Decrypt(SelectedConnection.EncryptedConnectionString);
        }
        catch
        {
            StatusMessage = "Error: invalid connection string.";
            IsLoading = false;
            return;
        }

        try
        {
            var filter = Filter.BuildFilter();
            var results = await _repository.GetLogsAsync(
                connectionString,
                SelectedConnection.TableName,
                SelectedConnection.MaxRows,
                filter,
                SelectedConnection.CommandTimeout,
                token);

            if (token.IsCancellationRequested) return;

            Logs.Clear();
            foreach (var entry in results)
                Logs.Add(entry);

            ErrorMessage = string.Empty;
            LastRefresh = $"Last update: {DateTime.Now:HH:mm:ss}";
            StatusMessage = Logs.Count == 0
                ? "No data matching filters."
                : $"{Logs.Count} entrie(s)";
        }
        catch (OperationCanceledException)
        {
            // Superseded by a newer request — ignore
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            StatusMessage = "Error while loading.";
        }
        finally
        {
            if (!token.IsCancellationRequested)
                IsLoading = false;
        }
    }
}
