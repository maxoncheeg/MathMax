using MathMax.Desktop.ViewModels.Commands;
using MathMax.Desktop.ViewModels.Navigation;

namespace MathMax.Desktop.ViewModels;

public class MainViewModel : BaseViewModel
{
    private string _title = "MathMax";

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public MainViewModel(INavigationService service)
    {
        service.ViewChanged += title =>
        {
            Title = title;
            Console.WriteLine($"Title: {title}");
        };
    }

    public override string ToString()
    {
        return "MathMax";
    }
}