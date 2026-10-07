using System.Windows.Media.Imaging;
using MathMax.Infrastructure.Pi;

namespace MathMax.Desktop.Models;

public interface IPiVisualizer
{
    public WriteableBitmap Bitmap { get; }
    
    public void AddPoint(PiPoint point);
    public void AddPoints(List<PiPoint> points);
    public void DrawCircle();
    public void Flush();
    public void Clear();
}