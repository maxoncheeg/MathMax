using System.Windows;
using System.Windows.Controls;

namespace MathMax.Desktop.Views;

public partial class StartupView : UserControl
{
    public StartupView()
    {
        InitializeComponent();
        
        Console.WriteLine(new Random().NextDouble());
    }
}