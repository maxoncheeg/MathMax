using System.Net.Sockets;
using System.Text;

namespace MathMax.Infrastructure.Network;

public class UdpReceiver : IUdpReceiver
{
    public int Port { get; set; } = 5000;

    public async Task WaitForMessageAsync(Action<string> onReceiving, CancellationToken token)
    {
        try
        {
            var udpClient = new UdpClient(Port);
            
            while (!token.IsCancellationRequested)
            {
                var result = await udpClient.ReceiveAsync(token);
                var message = Encoding.UTF8.GetString(result.Buffer);

                onReceiving.Invoke(message);
                
                break;
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("CANCELLED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UDP ERROR: {ex}");
        }
    }
}