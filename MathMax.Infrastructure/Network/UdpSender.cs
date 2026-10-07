using System.Net.Sockets;
using System.Text;

namespace MathMax.Infrastructure.Network;

public class UdpSender : IUdpSender
{
    public async Task SendUdpMessageAsync(string message, string address, int port, CancellationToken token)
    {
        var udp = new UdpClient(port);
        udp.EnableBroadcast = true;

        var bytes = Encoding.UTF8.GetBytes(message);

        await udp.SendAsync(bytes, address, port, token);
    }
}