namespace MathMax.Infrastructure.Network;

public interface IUdpSender
{
    public Task SendUdpMessageAsync(string message, string address, int port, CancellationToken token);
}