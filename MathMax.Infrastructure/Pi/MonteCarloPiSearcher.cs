using System.Diagnostics;

namespace MathMax.Infrastructure.Pi;

public class MonteCarloPiSearcher : IPiSearcher
{
    private const long RenderUpdateIntervalMs = 100;

    private const string Pi =
        "3,1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679";

    public event Action<double>? PercentChanged;
    public event Action<PiPoint>? PointAppeared;
    public event Action<PiResult>? SearchCompleted;
    public event Action? SearchStarted;

    public async Task<PiResult> SearchAsync(long points,
        CancellationToken ct)
    {
        SearchStarted?.Invoke();
        
        long inside = 0;
        long processed = 0;
        var lastRenderTime = Stopwatch.GetTimestamp();
        var renderLock = new Lock();

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        await Task.Run(() =>
        {
            Parallel.For(0, points,
                () => (Inside: 0L, Count: 0L, Rng: new Random()),
                (i, loopState, local) =>
                {
                    double x = local.Rng.NextDouble();
                    double y = local.Rng.NextDouble();

                    bool isPointInside = x * x + y * y < 1.0;

                    if (isPointInside) local.Inside++;
                    local.Count++;

                    long now = Stopwatch.GetTimestamp();
                    double elapsedMs = (now - lastRenderTime) * 100000.0 / Stopwatch.Frequency;


                    if (elapsedMs >= RenderUpdateIntervalMs)
                    {
                        PointAppeared?.Invoke(new PiPoint(x, y, isPointInside));
                        lock (renderLock)
                        {
                            lastRenderTime = now;
                        }
                    }
                    
                    return local;
                },
                local =>
                {
                    Interlocked.Add(ref inside, local.Inside);
                    long currentProcessed = Interlocked.Add(ref processed, local.Count);

                    if (points > 0 && currentProcessed % (points / 100) < local.Count)
                        PercentChanged?.Invoke((double)currentProcessed / points * 100.0);
                });
        }, ct);

        PercentChanged?.Invoke(100);
        stopwatch.Stop();

        var result = new PiResult(4.0 * inside / processed, inside, processed, stopwatch.ElapsedMilliseconds);
        
        SearchCompleted?.Invoke(result);
        
        return result;
    }
}