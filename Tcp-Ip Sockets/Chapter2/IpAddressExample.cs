using System.Net;

namespace Tcp_Ip_Sockets.Chapter2;

internal static class IpAddressExample
{
    public static void PrintHostInfo(string host)
    {
        try
        {
            // Attempt to resolve DNS for given host or address
            var hostInfo = Dns.GetHostEntry(host);

            // Display the primary host name
            Console.WriteLine("\tCanonical Name: " + hostInfo.HostName);

            // Display list of IP addresses for this host
            Console.Write("\tIP Addresses: ");
            foreach (var ipaddr in hostInfo.AddressList) Console.Write(ipaddr + " ");
            Console.WriteLine();

            // Display list of alias names for this host
            Console.Write("\tAliases: ");
            foreach (var alias in hostInfo.Aliases) Console.Write(alias + " ");
            Console.WriteLine("\n");
        }
        catch (Exception e)
        {
            Console.WriteLine("\tUnable to resolve host: " + host + "\n");
        }
    }
}