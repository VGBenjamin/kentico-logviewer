using System.Windows;
using KenticoLogViewer.Models;
using KenticoLogViewer.Services;
using KenticoLogViewer.ViewModels;

namespace KenticoLogViewer.Views;

public partial class LogDetailWindow : Window
{
    public LogDetailWindow(ILogRepository repository, string connectionString, string tableName, int eventId, int commandTimeout = 30)
    {
        InitializeComponent();
        var vm = new LogDetailViewModel(repository);
        DataContext = vm;
        Loaded += async (_, _) => await vm.LoadAsync(connectionString, tableName, eventId, commandTimeout);
    }
}
