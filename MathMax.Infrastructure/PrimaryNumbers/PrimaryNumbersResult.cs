namespace MathMax.Infrastructure.PrimaryNumbers;

public record PrimaryNumbersResult(decimal Amount, decimal Max, decimal Min, long Milliseconds) : IPrimaryNumbersResult;