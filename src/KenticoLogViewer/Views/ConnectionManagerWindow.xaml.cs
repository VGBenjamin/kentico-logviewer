using System.Windows;
using KenticoLogViewer.Services;
using KenticoLogViewer.ViewModels;

namespace KenticoLogViewer.Views;

public partial class ConnectionManagerWindow : Window
{
    public ConnectionManagerWindow(IConnectionStore connectionStore, ILogRepository repository)
    {
        InitializeComponent();
        DataContext = new ConnectionManagerViewModel(connectionStore, repository);
    }

    private void OnDeleteClick(object sender, RoutedEventArgs e)
    {
        var vm = (ConnectionManagerViewModel)DataContext;
        if (vm.SelectedConnection is null) return;

        var result = MessageBox.Show(
            $"Supprimer la connexion « {vm.SelectedConnection.Name} » ?",
            "Confirmer la suppression",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
            vm.DeleteCommand.Execute(null);
    }
}
