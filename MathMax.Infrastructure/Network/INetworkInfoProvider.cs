namespace MathMax.Infrastructure.Network;

public interface INetworkInfoProvider
{
    public NetworkInfo? GetCurrentNetworkInfo();
}