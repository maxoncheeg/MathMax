using MathMax.Desktop.ViewModels.Commands;
using MathMax.Desktop.ViewModels.Navigation;
using MathMax.Infrastructure.Network;

namespace MathMax.Desktop.ViewModels;

public class StartupViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;

    private string _ipAddress = "0.0.0.0";
    private string _port = "5000";

    public string IpAddress
    {
        get =>  _ipAddress;
        set => SetProperty(ref _ipAddress, value);
    }
    
    public string Port
    {
        get =>  _port;
        set => SetProperty(ref _port, value);
    }
    
    public RelayCommand GoToHost => new RelayCommand(() =>
    {
        _navigationService.NavigateTo(typeof(HostViewModel));
    });
    
    public RelayCommand GoToTask => new RelayCommand(() =>
    {
        _navigationService.NavigateTo(typeof(HostViewModel));
    });

    public StartupViewModel(INavigationService navigationService, INetworkInfoProvider networkInfoProvider)
    {
        _navigationService = navigationService;

        var ip = networkInfoProvider.GetCurrentNetworkInfo();
        
        if (ip is { IpAddress: not null })
            IpAddress = ip.IpAddress;
    }
    
    public override string ToString()
    {
        return "MathMax. Настройка IP-рассылки";
    }
}