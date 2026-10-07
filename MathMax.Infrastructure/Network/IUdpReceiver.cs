namespace MathMax.Infrastructure.Network;

public interface IUdpReceiver
{
    public int Port { get; set; }
    
    public Task WaitForMessageAsync(Action<string> onReceiving, CancellationToken token);
}