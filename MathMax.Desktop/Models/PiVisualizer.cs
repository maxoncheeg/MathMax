using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MathMax.Infrastructure.Pi;

namespace MathMax.Desktop.Models;

public class PiVisualizer : IPiVisualizer
{
    private readonly int _size;
    private readonly uint[] _pixelBuffer;
    private readonly Lock _lock = new();

    public uint InsideColor { get; set; } = 0x00d656u;
    public uint OutsideColor { get; set; } = 0xF22EA4u;
    public uint LineColor { get; set; } = 0x878bffu;
    public uint FillColor { get; set; } = 0x0f0f0fu;

    public WriteableBitmap Bitmap { get; private set; }

    public PiVisualizer()
    {
        _size = 500;
        Bitmap = new WriteableBitmap(_size, _size, 96D, 96D, PixelFormats.Bgr32, null);
        _pixelBuffer = new uint[_size * _size];

        Clear();
    }
    
    public PiVisualizer(int size)
    {
        _size = size;
        Bitmap = new WriteableBitmap(_size, _size, 96D, 96D, PixelFormats.Bgr32, null);
        _pixelBuffer = new uint[_size * _size];

        Clear();
    }

    public void AddPoint(PiPoint point)
    {
        lock (_lock)
        {
            int x = (int)(point.X * _size);
            int y = (int)(point.Y * _size);
            y = _size - y;
            int index = y * _size + x;

            if (index >= _pixelBuffer.Length || index < 0) return;

            _pixelBuffer[index] = point.IsInside ? InsideColor : OutsideColor;
        }
    }

    public void AddPoints(List<PiPoint> points)
    {
        lock (_lock)
        {
            foreach (var point in points)
            {
                int x = (int)(point.X * _size);
                int y = (int)(point.Y * _size);
                int index = y * _size + x;

                _pixelBuffer[index] = point.IsInside ? InsideColor : OutsideColor;
            }
        }
    }

    public void DrawCircle()
    {
        lock (_lock)
        {
            Clear();

            int cx = 0;
            int cy = _size - 1;
            double r = _size;

            for (double i = 270; i < 360; i += 0.175)
            {
                double angle = i * Math.PI / 180;

                int x = cx + (int)(r * Math.Cos(angle));
                int y = cy + (int)(r * Math.Sin(angle));
                int index = y * _size + x;

                if (index >= _pixelBuffer.Length || index < 0) continue;

                _pixelBuffer[index] = LineColor;
            }

            for (int x = 0; x < _size; x++)
            {
                int index = _size * (_size - 1) + x;
                _pixelBuffer[index] = LineColor;
            }

            for (int y = 0; y < _size - 1; y++)
            {
                int index = _size * y;
                _pixelBuffer[index] = LineColor;
            }

            Flush();
        }
    }

    public void Flush()
    {
        lock (_lock)
        {
            Bitmap.WritePixels(new Int32Rect(0, 0, _size, _size),
                _pixelBuffer, _size * 4, 0);
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            Array.Fill(_pixelBuffer, FillColor);
        }
    }
}