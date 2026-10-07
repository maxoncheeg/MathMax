using MathMax.Desktop.ViewModels.Commands;
using MathMax.Desktop.ViewModels.Navigation;
using MathMax.Infrastructure.Network;

namespace MathMax.Desktop.ViewModels;

public class StartupViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IUdpSender _udpSender;
    private readonly IUdpReceiver _udpReceiver;
    private readonly IPiHost _piHost;

    private string _trueIpAddress= "0.0.0.0";
    
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
    
    public RelayCommand GoToHost => new RelayCommand(async () =>
    {
        _piHost.Port = int.Parse(Port);
        await _udpSender.SendUdpMessageAsync(_trueIpAddress, IpAddress, _piHost.Port);
        
        _navigationService.NavigateTo(typeof(HostViewModel));
    });
    
    public RelayCommand GoToTask => new RelayCommand(() =>
    {
        _udpReceiver.Port = int.Parse(Port); 
        _navigationService.NavigateTo(typeof(TaskViewModel));
    });

    public StartupViewModel(INavigationService navigationService, INetworkInfoProvider networkInfoProvider, IUdpSender udpSender, IUdpReceiver udpReceiver, IPiHost piHost)
    {
        _navigationService = navigationService;
        _udpSender = udpSender;
        _udpReceiver = udpReceiver;
        _piHost = piHost;

        var ip = networkInfoProvider.GetCurrentNetworkInfo();
        
        if (ip is { IpAddress: not null })
        {
            _trueIpAddress = IpAddress = ip.IpAddress;
            IpAddress = IpAddress[..(IpAddress.LastIndexOf('.') + 1)] + "255"; // full lol!
        }
    }
    
    public override string ToString()
    {
        return "MathMax. Настройка IP-рассылки";
    }
}