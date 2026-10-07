using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using MathMax.Desktop.ViewModels.Commands;
using MathMax.Desktop.ViewModels.Navigation;
using MathMax.Infrastructure.Network;
using MathMax.Infrastructure.Pi;
using Microsoft.Extensions.DependencyInjection;

namespace MathMax.Desktop.ViewModels;

public class TaskViewModel : BaseViewModel
{
 private INavigationService _navigationService;
    private readonly IPiSearcher _piSearcher;
    private IPiClient _piClient;
    private IUdpReceiver _udpReceiver;
    
    private bool _inProgress = false;
    private CancellationTokenSource _searcherCts = new();
    private CancellationTokenSource _hostCts = new();


    private string _status = "Ожидаем вызова по UDP...";
    private long _points = 1_000_000_00;
    private object _piVisual;

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }
    
    public object PiVisual
    {
        get => _piVisual;
        set => SetProperty(ref _piVisual, value);
    }

    public long Points
    {
        get => _points;
        set => SetProperty(ref _points, value);
    }

    public TaskViewModel(INavigationService navigationService, IPiSearcher piSearcher, IServiceProvider serviceProvider, IUdpReceiver udpReceiver)
    {
        _navigationService = navigationService;
        _piSearcher = piSearcher;
        _udpReceiver = udpReceiver;

        _udpReceiver.WaitForMessageAsync((x) => OnMessageReceivedAsync(x), _hostCts.Token);
        
        _piVisual = serviceProvider.GetRequiredService<PiVisualViewModel>();
    }

    public override string ToString()
    {
        return "MathMax. Host";
    }

    private async Task OnMessageReceivedAsync(string message)
    {
        _piClient = new PiClient(message, _udpReceiver.Port);
        Application.Current.Dispatcher.Invoke(() =>
        {
            Status = "Помогаем " + message + " решить задачу...";
        });

        await _piClient.ListenAsync(async (message) =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Points = long.Parse(message);
                Status = "Решаем локальную задачу...";
            });
            
            
            var result = await _piSearcher.SearchAsync(Points, _searcherCts.Token);

            await _piClient.SendAsync(Encoding.UTF8.GetBytes($"{result.Inside} {result.Processed}"));
            
            await _hostCts.CancelAsync();
        });
        
        Application.Current.Dispatcher.Invoke(() =>
        {
            Status = "Задача решена...";
        });
        
        _hostCts = new CancellationTokenSource();
    }
}