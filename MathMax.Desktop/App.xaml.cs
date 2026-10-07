using System.Configuration;
using System.Data;
using System.Windows;
using MathMax.Desktop.ViewModels;
using MathMax.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;

namespace MathMax.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static IServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        _serviceProvider = CompositionRoot.ConfigureServices();
        
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.DataContext = _serviceProvider.GetRequiredService<MainViewModel>();
        
        mainWindow.Show();
    }
}