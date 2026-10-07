using System.Windows.Controls;

namespace MathMax.Desktop.ViewModels.Navigation;

public interface INavigationService
{
    event Action<string> ViewChanged; 
    void NavigateTo<TViewModel>() where TViewModel : BaseViewModel;
    void SetContentHost(ContentControl contentHost);
    void NavigateTo(Type viewModelType);
    void GoBack();
}