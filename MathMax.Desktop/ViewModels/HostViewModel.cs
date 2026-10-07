
using MathMax.Desktop.ViewModels.Commands;
using MathMax.Desktop.ViewModels.Navigation;
using MathMax.Infrastructure.Pi;
using Microsoft.Extensions.DependencyInjection;

namespace MathMax.Desktop.ViewModels;

public class HostViewModel : BaseViewModel
{
    private INavigationService _navigationService;
    private readonly IPiSearcher _piSearcher;

    private bool _inProgress = false;
    private CancellationTokenSource _cancellationTokenSource = new();
    

    private long _points = 1_000_000_00;

    private object _piVisual;

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

    public RelayCommand StartCommand { get; } 

    public HostViewModel(INavigationService navigationService, IPiSearcher piSearcher, IServiceProvider serviceProvider)
    {
        _navigationService = navigationService;
        _piSearcher = piSearcher;
        
        StartCommand = new RelayCommand(_ => StartPiSearch(), _ => !_inProgress);
        
        _piVisual = serviceProvider.GetRequiredService<PiVisualViewModel>();
    }
    
    public override string ToString()
    {
        return "MathMax. Host";
    }
    
    private async Task StartPiSearch()
    {
        _inProgress = true;
        
        Console.WriteLine("Started");
        
        await Task.Run(async () =>
        {
            var result = await _piSearcher.SearchAsync(_points, _cancellationTokenSource.Token);
        });
        
        Console.WriteLine("Finished");
        
        _inProgress = false;
    }
}