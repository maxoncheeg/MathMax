using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MathMax.Infrastructure.Pi;

namespace MathMax.Infrastructure.Network;

public class PiHost : IPiHost
{
    public event Action<int, PiResult>? TaskPartReceived;
    public event Action<string>? HelperConnected;

    private readonly ConcurrentDictionary<string, TcpClient> _clients = new();

    private bool _stopRequested = false;
    
    
    public async Task StartAsync(string address, int port, CancellationToken cancellationToken = default)
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        
        Console.WriteLine($"Сервер запущен на порту {port}");

        try
        {
            while (!_stopRequested || !cancellationToken.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync(cancellationToken);
                var clientId = Guid.NewGuid().ToString("N");
                
                _clients.TryAdd(clientId, client);
                HelperConnected?.Invoke((client.Client.RemoteEndPoint as IPEndPoint)?.Address.ToString() ?? "unknown");
                
                Console.WriteLine($"Клиент подключился: {clientId}. Всего клиентов: {_clients.Count}");
                
                _ = Task.Run(() => HandleClientAsync(clientId, client), cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Сервер остановлен");
        }
        finally
        {
            listener.Stop();
        }

        _stopRequested = false;
    }
    
    private async Task HandleClientAsync(string clientId, TcpClient client)
    {
        using var stream = client.GetStream();
        var buffer = new byte[4096];

        try
        {
            while (client.Connected)
            {
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break; // Клиент отключился

                var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"[{clientId}] Получено: {message}");
                
                var response = $"Echo: {message}";
                var responseBytes = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);

                // await BroadcastAsync(clientId, message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка с клиентом {clientId}: {ex.Message}");
        }
        finally
        {
            _clients.TryRemove(clientId, out _);
            client.Close();
            Console.WriteLine($"Клиент отключился: {clientId}. Осталось: {_clients.Count}");
        }
    }
    
    private async Task BroadcastAsync(string senderId, string message)
    {
        var broadcastMessage = $"[{senderId}]: {message}";
        var bytes = Encoding.UTF8.GetBytes(broadcastMessage);

        foreach (var kvp in _clients)
        {
            if (kvp.Key != senderId && kvp.Value.Connected)
            {
                try
                {
                    var stream = kvp.Value.GetStream();
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                }
                catch
                {
                    // Игнорируем ошибки отправки
                }
            }
        }
    }

    public Task SendTaskToEachHelperAsync(long points, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync()
    {
        _stopRequested = true;
        
        return Task.CompletedTask;
    }
    
    public void Dispose()
    {
        _stopRequested = true;
    }

    public async ValueTask DisposeAsync()
    {
        _stopRequested = true;
    }
}