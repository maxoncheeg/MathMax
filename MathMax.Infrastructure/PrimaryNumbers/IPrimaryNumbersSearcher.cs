namespace MathMax.Infrastructure.PrimaryNumbers;

public interface IPrimaryNumbersSearcher
{
    public event EventHandler<int> PercentChanged;
    public IPrimaryNumbersResult Search(decimal number);
}