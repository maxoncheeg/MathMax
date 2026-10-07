using System.Windows;
using MathMax.Desktop.ViewModels;
using MathMax.Desktop.ViewModels.Navigation;

namespace MathMax.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow(INavigationService navigationService)
    {
        InitializeComponent();
        
        navigationService.SetContentHost(this.MainContentHost);

        Loaded += (sender, args) =>
        {
            navigationService.NavigateTo(typeof(StartupViewModel));
        };
    }

    public MainWindow()
    {
        InitializeComponent();
    }
}