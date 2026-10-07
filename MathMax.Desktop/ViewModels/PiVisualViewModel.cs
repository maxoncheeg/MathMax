using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;
using MathMax.Desktop.Models;
using MathMax.Infrastructure.Network;
using MathMax.Infrastructure.Pi;

namespace MathMax.Desktop.ViewModels;

public class PiVisualViewModel : BaseViewModel
{
    private const string RightPi =
        "3,1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679";

    private IPiVisualizer _piVisualizer;

    private long _lastRenderTime;

    private double _percent = 0;

    private long _insidePoints = 0;
    private long _outsidePoints = 0;
    private long _milliseconds = 0;
    private double _pi = 0;

    private int _wrongPiStartIndex = 0;
    private int _wrongPiLength = 0;
    
    private Lock _uiLock = new ();

    public WriteableBitmap Bitmap => _piVisualizer.Bitmap;

    public double Percent
    {
        get => _percent;
        set => SetProperty(ref _percent, value);
    }

    public long InsidePoints
    {
        get => _insidePoints;
        set => SetProperty(ref _insidePoints, value);
    }

    public long OutsidePoints
    {
        get => _outsidePoints;
        set => SetProperty(ref _outsidePoints, value);
    }

    public long Milliseconds
    {
        get => _milliseconds;
        set => SetProperty(ref _milliseconds, value);
    }

    public double Pi
    {
        get => _pi;
        set => SetProperty(ref _pi, value);
    }

    public int WrongPiStartIndex
    {
        get => _wrongPiStartIndex;
        set => SetProperty(ref _wrongPiStartIndex, value);
    }

    public int WrongPiLength
    {
        get => _wrongPiLength;
        set => SetProperty(ref _wrongPiLength, value);
    }


    public PiVisualViewModel(IPiVisualizer piVisualizer, IPiSearcher piSearcher, IPiHost piHost)
    {
        _piVisualizer = piVisualizer;

        piSearcher.SearchStarted += PiSearcherOnSearchStarted;
        piSearcher.PercentChanged += PiSearcherOnPercentChanged;
        piSearcher.PointAppeared += PiSearcherOnPointAppeared;
        piSearcher.SearchCompleted += PiSearcherOnSearchCompleted;

        _piVisualizer.DrawCircle();
        
        piHost.TaskPartReceived += PiHostOnTaskPartReceived;
    }

    private void PiHostOnTaskPartReceived(int arg1, PiResult arg2)
    {
        Application.Current.Dispatcher.Invoke(() =>
            {
                _uiLock.Enter();
                
                InsidePoints = InsidePoints + arg2.Inside;
                
                OutsidePoints = OutsidePoints + arg2.Processed - arg2.Inside;
                Pi = 4.0D * (double)InsidePoints / (InsidePoints + OutsidePoints);
                
                WrongPiStartIndex = 0;
                
                string pi = Pi.ToString();
                int wrongPiLength = 0;

                for (int i = 0; i < pi.Length; i++)
                {
                    if (pi[i] != RightPi[i] && WrongPiStartIndex == 0)
                    {
                        WrongPiStartIndex = i;
                    }
                
                    if(WrongPiStartIndex != 0)
                        wrongPiLength++;
                }
                WrongPiLength = wrongPiLength;
                
                _uiLock.Exit();
            }
        );
    }

    private void PiSearcherOnSearchCompleted(PiResult result)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            _uiLock.Enter();
            
            Milliseconds = result.TotalMilliseconds;
            InsidePoints = InsidePoints + result.Inside;
            OutsidePoints = OutsidePoints + result.Processed - result.Inside;
            
            Pi = 4.0D * (double)InsidePoints / (InsidePoints + OutsidePoints);

            WrongPiStartIndex = 0;

            string pi = Pi.ToString();
            int wrongPiLength = 0;

            for (int i = 0; i < pi.Length; i++)
            {
                if (pi[i] != RightPi[i] && WrongPiStartIndex == 0)
                {
                    WrongPiStartIndex = i;
                }
                
                if(WrongPiStartIndex != 0)
                    wrongPiLength++;
            }
            WrongPiLength = wrongPiLength;
            
            _uiLock.Exit();
        });
    }

    private void PiSearcherOnPointAppeared(PiPoint point)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            _piVisualizer.AddPoint(point);
            
            var time = Stopwatch.GetTimestamp();

            if (time - _lastRenderTime > 10000)
            {
                _piVisualizer.Flush();
                _lastRenderTime = Stopwatch.GetTimestamp();
            }
        });
    }

    private void PiSearcherOnPercentChanged(double percent)
    {
        Application.Current.Dispatcher.Invoke(() => { Percent = percent; });
    }

    private void PiSearcherOnSearchStarted()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Percent = 0;
            Pi = 0;
            Milliseconds = 0;
            InsidePoints = 0;
            OutsidePoints = 0;
            _lastRenderTime = Stopwatch.GetTimestamp();
            _piVisualizer.DrawCircle();
        });
    }

    public override string ToString()
    {
        return "MathMax. PiVisual";
    }
}