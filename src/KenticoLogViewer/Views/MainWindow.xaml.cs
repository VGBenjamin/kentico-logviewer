using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KenticoLogViewer.Helpers;
using KenticoLogViewer.Models;
using KenticoLogViewer.Services;
using KenticoLogViewer.ViewModels;

namespace KenticoLogViewer.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm;
    private readonly ILogRepository _repository;
    private readonly IConnectionStore _connectionStore;

    public MainWindow(MainViewModel viewModel, ILogRepository repository, IConnectionStore connectionStore)
    {
        InitializeComponent();
        _vm = viewModel;
        _repository = repository;
        _connectionStore = connectionStore;
        DataContext = _vm;

        // Ctrl+C shortcut for copy
        CommandBindings.Add(new CommandBinding(
            ApplicationCommands.Copy,
            (_, _) => _vm.CopySelectedCommand.Execute(null)));
    }

    private void OnManageConnectionsClick(object sender, RoutedEventArgs e)
    {
        var window = new ConnectionManagerWindow(_connectionStore, _repository) { Owner = this };
        window.ShowDialog();
        _vm.ReloadConnectionsCommand.Execute(null);
    }

    private void OnGridDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (LogGrid.SelectedItem is not EventLogEntry entry) return;
        if (_vm.SelectedConnection is null) return;

        string connectionString;
        try
        {
            connectionString = DpapiHelper.Decrypt(_vm.SelectedConnection.EncryptedConnectionString);
        }
        catch
        {
            return;
        }

        var detail = new LogDetailWindow(
            _repository,
            connectionString,
            _vm.SelectedConnection.TableName,
            entry.EventID,
            _vm.SelectedConnection.CommandTimeout)
        {
            Owner = this
        };
        detail.ShowDialog();
    }

    private void OnGridSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _vm.SelectedLogs.Clear();
        foreach (EventLogEntry item in LogGrid.SelectedItems)
            _vm.SelectedLogs.Add(item);
    }
}
