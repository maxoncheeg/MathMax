using System.Net.Sockets;
using System.Text;
using MathMax.Infrastructure.Network;
using MathMax.Infrastructure.Pi;

namespace MathMax.Mobile;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();



        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        var receiver = new UdpReceiver();
        receiver.Port = 5000;
        
        CounterBtn.Text = "НАЧИНАЕМ РАБОТУ";
        SemanticScreenReader.Announce(CounterBtn.Text);
        
        Task.Run(() => receiver.WaitForMessageAsync((x) => OnMessageReceived(x),  CancellationToken.None));
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
    
    private async Task OnMessageReceived(string ip)
    {
        var client = new PiClient(ip, 5000);

        var cts = new CancellationTokenSource();
        string text = "0";
        
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            CounterBtn.Text = "РАБОТАЕМ НА " + ip;
            SemanticScreenReader.Announce(CounterBtn.Text);
        });
        
        await client.ListenAsync(async (message) =>
        {
            var points = long.Parse(message);

            var searcher = new MonteCarloPiSearcher();

            var result = await Task.Run(async () => await searcher.SearchAsync(points, CancellationToken.None), cts.Token);
            text = result.Pi.ToString();
            
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                CounterBtn.Text = text;
                SemanticScreenReader.Announce(CounterBtn.Text);
            });
            
            await client.SendAsync(Encoding.UTF8.GetBytes($"{result.Inside} {result.Processed}"));
        });
        

    }
}