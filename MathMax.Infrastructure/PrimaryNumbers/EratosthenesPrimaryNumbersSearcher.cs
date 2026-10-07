using System.Collections;
using System.Diagnostics;

namespace MathMax.Infrastructure.PrimaryNumbers;

public class EratosthenesPrimaryNumbersSearcher : IPrimaryNumbersSearcher
{
    public event EventHandler<int>? PercentChanged;

    public IPrimaryNumbersResult Search(decimal number)
    {
        OnPercentChanged(0);
        
        BitArray array = new BitArray((int)number, true);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        for (int i = 2; i < array.Count; i++)
        {
            if (array[i])
            {
                var j = i + i;
                while (j < array.Count)
                {
                    array[j] = false;
                    j += i;
                }
            }
            
            
        }
        stopwatch.Stop();
        
        return new PrimaryNumbersResult(0, 0, 0, stopwatch.ElapsedMilliseconds);
    }

    private void OnPercentChanged(int percent) =>
        PercentChanged?.Invoke(this, percent);
}