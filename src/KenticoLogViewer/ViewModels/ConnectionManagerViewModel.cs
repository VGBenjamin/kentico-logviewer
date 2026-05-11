using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KenticoLogViewer.Helpers;
using KenticoLogViewer.Models;
using KenticoLogViewer.Services;

namespace KenticoLogViewer.ViewModels;

public partial class ConnectionManagerViewModel : ObservableObject
{
    private readonly IConnectionStore _store;
    private readonly ILogRepository _repository;
    private readonly List<ConnectionConfig> _connections;

    public ObservableCollection<ConnectionConfig> Connections { get; }

    [ObservableProperty] private ConnectionConfig? _selectedConnection;
    [ObservableProperty] private bool _isEditing;

    // Edit form fields
    [ObservableProperty] private string _editName = string.Empty;
    [ObservableProperty] private string _editConnectionString = string.Empty;
    [ObservableProperty] private string _editTableName = "CMS_EventLog";
    [ObservableProperty] private int _editMaxRows = 500;
    [ObservableProperty] private int _editCommandTimeout = 30;

    // Test connection feedback
    [ObservableProperty] private string _testResultMessage = string.Empty;
    [ObservableProperty] private bool _isTestingConnection;

    private ConnectionConfig? _editingTarget;

    public ConnectionManagerViewModel(IConnectionStore store, ILogRepository repository)
    {
        _store = store;
        _repository = repository;
        _connections = store.Load().ToList();
        Connections = new ObservableCollection<ConnectionConfig>(_connections);
    }

    [RelayCommand]
    private void StartAdd()
    {
        _editingTarget = null;
        EditName = string.Empty;
        EditConnectionString = string.Empty;
        EditTableName = "CMS_EventLog";
        EditMaxRows = 500;
        EditCommandTimeout = 30;
        TestResultMessage = string.Empty;
        IsEditing = true;
    }

    [RelayCommand]
    private void StartEdit()
    {
        if (SelectedConnection is null) return;
        _editingTarget = SelectedConnection;
        EditName = SelectedConnection.Name;
        EditConnectionString = DpapiHelper.Decrypt(SelectedConnection.EncryptedConnectionString);
        EditTableName = SelectedConnection.TableName;
        EditMaxRows = SelectedConnection.MaxRows;
        EditCommandTimeout = SelectedConnection.CommandTimeout;
        TestResultMessage = string.Empty;
        IsEditing = true;
    }

    [RelayCommand(CanExecute = nameof(CanSaveEdit))]
    private void SaveEdit()
    {
        if (_editingTarget is null)
        {
            var config = new ConnectionConfig
            {
                Id = Guid.NewGuid().ToString(),
                Name = EditName.Trim(),
                EncryptedConnectionString = DpapiHelper.Encrypt(EditConnectionString),
                TableName = EditTableName.Trim(),
                MaxRows = EditMaxRows,
                CommandTimeout = EditCommandTimeout,
            };
            _connections.Add(config);
            Connections.Add(config);
            SelectedConnection = config;
        }
        else
        {
            _editingTarget.Name = EditName.Trim();
            _editingTarget.EncryptedConnectionString = DpapiHelper.Encrypt(EditConnectionString);
            _editingTarget.TableName = EditTableName.Trim();
            _editingTarget.MaxRows = EditMaxRows;
            _editingTarget.CommandTimeout = EditCommandTimeout;

            var idx = Connections.IndexOf(_editingTarget);
            if (idx >= 0)
            {
                Connections.RemoveAt(idx);
                Connections.Insert(idx, _editingTarget);
                SelectedConnection = _editingTarget;
            }
        }

        Persist();
        IsEditing = false;
    }

    private bool CanSaveEdit() =>
        !string.IsNullOrWhiteSpace(EditName) && !string.IsNullOrWhiteSpace(EditConnectionString);

    partial void OnEditNameChanged(string value) => SaveEditCommand.NotifyCanExecuteChanged();
    partial void OnEditConnectionStringChanged(string value) => SaveEditCommand.NotifyCanExecuteChanged();

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditing = false;
        TestResultMessage = string.Empty;
    }

    [RelayCommand]
    private void Delete()
    {
        if (SelectedConnection is null) return;
        _connections.Remove(SelectedConnection);
        Connections.Remove(SelectedConnection);
        SelectedConnection = Connections.FirstOrDefault();
        Persist();
    }

    [RelayCommand]
    private async Task TestConnection()
    {
        if (string.IsNullOrWhiteSpace(EditConnectionString)) return;
        IsTestingConnection = true;
        TestResultMessage = string.Empty;
        try
        {
            var table = string.IsNullOrWhiteSpace(EditTableName) ? "CMS_EventLog" : EditTableName.Trim();
            await _repository.TestConnectionAsync(EditConnectionString, table);
            TestResultMessage = "✔ Connection successful.";
        }
        catch (Exception ex)
        {
            TestResultMessage = $"✘ {ex.Message}";
        }
        finally
        {
            IsTestingConnection = false;
        }
    }

    private void Persist() => _store.Save(_connections);
}
