using System.Net.Sockets;
using System.Text;
using MathMax.Infrastructure.Pi;

namespace MathMax;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    private UdpClient _udpClient = null!;
    private CancellationTokenSource _tokenSource = null!;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.activity_main);

        _tokenSource = new CancellationTokenSource();
        // _ = ListenPortAsync(5000, _tokenSource.Token);
        var searcher = new MonteCarloPiSearcher();
        Task.Run(async () =>
        {
            var res = await searcher.SearchAsync(10000000, default);
            
            RunOnUiThread(() =>
                Toast.MakeText(this, res.Pi.ToString(), ToastLength.Short)!.Show());
        });
    }

    private async Task ListenPortAsync(int port, CancellationToken token)
    {
        try
        {
            Console.WriteLine("STARTED");

            RunOnUiThread(() =>
                Toast.MakeText(this, "Start", ToastLength.Short)!.Show());

            _udpClient = new UdpClient(port);

            RunOnUiThread(() =>
                Toast.MakeText(this, "UDP CREATED", ToastLength.Short)!.Show());

            while (!token.IsCancellationRequested)
            {
                RunOnUiThread(() =>
                    Toast.MakeText(this, "WAITING", ToastLength.Short)!.Show());

                var result = await _udpClient.ReceiveAsync(token);

                var message = Encoding.UTF8.GetString(result.Buffer);

                Console.WriteLine($"RECEIVED: {message}");

                RunOnUiThread(() =>
                    Toast.MakeText(this, message, ToastLength.Long)!.Show());
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("CANCELLED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UDP ERROR: {ex}");

            RunOnUiThread(() =>
                Toast.MakeText(
                    this,
                    $"ERROR: {ex.Message}",
                    ToastLength.Long
                )!.Show());
        }
    
    }

    protected override void OnDestroy()
    {
        _tokenSource?.Cancel();
        _udpClient?.Dispose();

        _tokenSource?.Dispose();

        base.OnDestroy();
    }
}