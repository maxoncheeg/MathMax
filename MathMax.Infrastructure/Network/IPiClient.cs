namespace MathMax.Infrastructure.Network;

public interface IPiClient
{
    public Task SendAsync(byte[] message);
    public Task ListenAsync(Action<string> onMessageReceived);
}