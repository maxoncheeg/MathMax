
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using MathMax.Desktop.ViewModels.Commands;
using MathMax.Desktop.ViewModels.Navigation;
using MathMax.Infrastructure.Network;
using MathMax.Infrastructure.Pi;
using Microsoft.Extensions.DependencyInjection;

namespace MathMax.Desktop.ViewModels;

public class HostViewModel : BaseViewModel
{
    private INavigationService _navigationService;
    private readonly IPiSearcher _piSearcher;
    private IPiHost _piHost;
    
    private bool _inProgress = false;
    private bool _taskSended = false;
    private CancellationTokenSource _searcherCts = new();
    private CancellationTokenSource _hostCts = new();
    private long _localPoints = 1_000_000_00;
    
    private Stopwatch _watch = new();


    private string _status = "Сканируем локальные помещения по UDP...";
    private ObservableCollection<string> _helpers = new();
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
    
    public ObservableCollection<string> Helpers
    {
        get => _helpers;
        set => SetProperty(ref _helpers, value);
    }

    public long Points
    {
        get => _points;
        set => SetProperty(ref _points, value);
    }
    
    public bool InProgress
    {
        get => _inProgress;
        set
        {
            if (SetProperty(ref _inProgress, value))
            {
                StartCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public RelayCommand StartCommand { get; }

    public HostViewModel(INavigationService navigationService, IPiSearcher piSearcher, IServiceProvider serviceProvider, IPiHost piHost)
    {
        _navigationService = navigationService;
        _piSearcher = piSearcher;
        _piHost = piHost;
        
        StartCommand = new RelayCommand(_ =>
        {
            StartPiSearch();
        }, _ => !InProgress);
        
        _piVisual = serviceProvider.GetRequiredService<PiVisualViewModel>();
        
        Console.WriteLine(piHost.Port);
        _piHost.HelperConnected += PiHostOnHelperConnected;
        _piHost.TaskPartReceived += PiHostOnTaskPartReceived;
        
        _ = _piHost.StartAsync(_hostCts.Token);
    }

    private int _counter = 0;
    private void PiHostOnTaskPartReceived(int arg1, PiResult arg2)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            _counter++;
            
            if (_counter == _piHost.Clients)
            {
                _watch.Stop();
                MessageBox.Show("Общее время: "+ _watch.ElapsedMilliseconds + " мс", "Задача выполнена!");
                _watch.Reset();
            }
        });
    }

    private void PiHostOnHelperConnected(string id)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Console.WriteLine($"NEW CONNECTION: {id}");
            _helpers.Add(id);
            Status = "Найдены помощники...";
        });
    }

    public override string ToString()
    {
        return "MathMax. Host";
    }
    
    private async Task StartPiSearch()
    {
        InProgress = true;
        
        _counter = 0;
        _watch.Start();
        _localPoints = await _piHost.SendTaskToEachHelperAsync(Points, _hostCts.Token);
        
        Status = "Рисуем точки...";
        Console.WriteLine("Started");
        
        await Task.Run(async () =>
        {
            var result = await _piSearcher.SearchAsync(_localPoints, _searcherCts.Token);
        });
        
        Console.WriteLine("Finished");
        
        InProgress = false;
        Status = "Процесс завершен...";
    }
}