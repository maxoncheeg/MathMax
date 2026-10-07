namespace MathMax.Infrastructure.Pi;

public interface IPiSearcher
{
    public event Action<double> PercentChanged;
    public event Action<PiPoint> PointAppeared;
    public event Action<PiResult> SearchCompleted;
    public event Action SearchStarted;
    
    public Task<PiResult> SearchAsync(long points, 
        CancellationToken ct);
}