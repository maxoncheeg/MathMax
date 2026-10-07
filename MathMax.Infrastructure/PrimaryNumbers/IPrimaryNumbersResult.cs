namespace MathMax.Infrastructure.PrimaryNumbers;

public interface IPrimaryNumbersResult
{
    public decimal Amount { get; }
    public decimal Max { get; }
    public decimal Min { get; }
    public long Milliseconds { get; }
}