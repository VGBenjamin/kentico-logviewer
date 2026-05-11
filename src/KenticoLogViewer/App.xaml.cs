using System.Windows;
using KenticoLogViewer.Services;
using KenticoLogViewer.ViewModels;
using KenticoLogViewer.Views;

namespace KenticoLogViewer;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var connectionStore = new ConnectionStore();
        var repository = new LogRepository();
        var mainVm = new MainViewModel(connectionStore, repository);

        var mainWindow = new MainWindow(mainVm, repository, connectionStore);
        mainWindow.Show();
    }
}

