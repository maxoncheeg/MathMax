using MathMax.Infrastructure.Pi;

namespace MathMax.Infrastructure.Network;

public interface IPiHost : IAsyncDisposable, IDisposable
{
    public event Action<int, PiResult> TaskPartReceived;
    public event Action<string> HelperConnected;
    
    public Task StartAsync(string address, int port, CancellationToken cancellationToken = default);
    public Task SendTaskToEachHelperAsync(long points, CancellationToken cancellationToken = default);
    public Task StopAsync();
}