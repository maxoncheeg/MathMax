using MathMax.Infrastructure.Pi;

namespace MathMax.Infrastructure.Network;

public interface IPiHost : IAsyncDisposable, IDisposable
{
    public event Action<int, PiResult> TaskPartReceived;
    public event Action<string> HelperConnected;
    
    public int Port { get; set; }
    public int Clients { get; }

    public Task StartAsync(CancellationToken cancellationToken = default);
    public Task<long> SendTaskToEachHelperAsync(long points, CancellationToken cancellationToken = default);
    public Task StopAsync();
}