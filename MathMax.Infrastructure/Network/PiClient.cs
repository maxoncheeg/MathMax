using System.Net.Sockets;
using System.Text;

namespace MathMax.Infrastructure.Network;

public class PiClient : IPiClient
{
    private readonly TcpClient _client;
    private NetworkStream _stream;
    private bool _isRunning;

    public PiClient(string host, int port)
    {
        _client = new TcpClient(host, port);
        _stream = _client.GetStream();
        _isRunning = true;
    }

    
    public async Task SendAsync(byte[] message)
    {
        if (!_client.Connected || !_isRunning)
            throw new InvalidOperationException("Клиент не подключен");

        await _stream.WriteAsync(message, 0, message.Length);
        await _stream.FlushAsync(); // Важно: гарантируем отправку данных
    }

    public async Task ListenAsync(Action<string> onMessageReceived)
    {
        var buffer = new byte[4096];

        while (_isRunning && _client.Connected)
        {
            try
            {
                var bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                onMessageReceived?.Invoke(message);
            }
            catch
            {
                break;
            }
        }
    }
}