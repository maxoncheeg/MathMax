using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace MathMax.Desktop.ViewModels.Navigation;

public class NavigationService(IServiceProvider serviceProvider) : INavigationService
{
    private readonly Stack<Type> _navigationHistory = new();
    private ContentControl _contentHost = null!;

    public void SetContentHost(ContentControl contentHost)
    {
        _contentHost = contentHost;
    }

    public event Action<string>? ViewChanged;

    public void NavigateTo<TViewModel>() where TViewModel : BaseViewModel
    {
        NavigateTo(typeof(TViewModel));
    }
    
    public void NavigateTo(Type viewModelType)
    {
        if (_contentHost.Content is FrameworkElement currentView)
        {
            _navigationHistory.Push(currentView.DataContext.GetType());
        }
        
        var viewModel = (BaseViewModel)serviceProvider.GetRequiredService(viewModelType);
        _contentHost.Content = viewModel;
        
        ViewChanged?.Invoke(viewModel.ToString());
    }
    
    public void GoBack()
    {
        if (_navigationHistory.Any())
        {
            var previousViewModelType = _navigationHistory.Pop();
            NavigateTo(previousViewModelType);
        }
    }
}