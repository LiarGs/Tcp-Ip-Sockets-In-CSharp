using System.Net;
using Tcp_Ip_Sockets.Chapter2;

namespace Tcp_Ip_Sockets;

internal abstract class Program
{
    private static void Main(string[] args)
    {
        // Get and print local host info
        try
        {
            Console.WriteLine("Local Host:");
            var localHostName = Dns.GetHostName();
            Console.WriteLine("\tHost Name: " + localHostName);

            IpAddressExample.PrintHostInfo(localHostName);
        }
        catch (Exception)
        {
            Console.WriteLine("Unable to resolve local host\n");
        }

        // Get and print info for hosts given on command line
        foreach (var arg in args)
        {
            Console.WriteLine(arg + ":");
            IpAddressExample.PrintHostInfo(arg);
        }
    }
}