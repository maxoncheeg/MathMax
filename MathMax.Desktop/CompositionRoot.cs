
using MathMax.Desktop.Models;
using MathMax.Desktop.ViewModels;
using MathMax.Desktop.ViewModels.Navigation;
using MathMax.Desktop.Views;
using MathMax.Infrastructure.Network;
using MathMax.Infrastructure.Pi;
using Microsoft.Extensions.DependencyInjection;
using HostViewModel = MathMax.Desktop.ViewModels.HostViewModel;

namespace MathMax.Desktop;

public static class CompositionRoot
{
    public static IServiceProvider ConfigureServices()
    {
        IServiceCollection collection = new ServiceCollection();

        collection.AddSingleton<MainViewModel>();
        collection.AddTransient<StartupViewModel>();
        collection.AddTransient<HostViewModel>();
        collection.AddTransient<TaskViewModel>();
        collection.AddTransient<PiVisualViewModel>();

        collection.AddTransient<MainWindow>();
        collection.AddTransient<StartupView>();
        collection.AddTransient<HostView>();
        collection.AddTransient<TaskView>();
        collection.AddTransient<PiVisualView>();
        
        collection.AddSingleton<INavigationService, NavigationService>();
        collection.AddSingleton<INetworkInfoProvider, NetworkInfoProvider>();
        collection.AddSingleton<IPiSearcher, MonteCarloPiSearcher>();
        collection.AddScoped<IPiHost, PiHost>();
        collection.AddScoped<IPiVisualizer, PiVisualizer>();
        
        return collection.BuildServiceProvider();
    }
}