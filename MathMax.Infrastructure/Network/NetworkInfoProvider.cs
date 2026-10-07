using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace MathMax.Infrastructure.Network;

public class NetworkInfoProvider : INetworkInfoProvider
{
    public NetworkInfo? GetCurrentNetworkInfo()
    {
        var interfaces = NetworkInterface.GetAllNetworkInterfaces();
        
        var wifiInterface = interfaces
            .FirstOrDefault(ni => 
                ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 &&
                ni.OperationalStatus == OperationalStatus.Up);
        
        if (wifiInterface == null)
            return null;
        
        var ipProps = wifiInterface.GetIPProperties();
        var ipv4Address = ipProps.UnicastAddresses
            .FirstOrDefault(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork);
        
        if (ipv4Address == null)
            return null;
        
        var gateway = ipProps.GatewayAddresses
            .FirstOrDefault(gw => gw.Address.AddressFamily == AddressFamily.InterNetwork)?
            .Address?.ToString();

        return new NetworkInfo(ipv4Address.Address.ToString(), ipv4Address.IPv4Mask.ToString());
    }
}