using System.Net;

namespace TcpIpSocketsLearn.Chapter2;

internal static class IpAddressExample
{
    public static void Example(string[] args)
    {
        // Get and print local host info
        try
        {
            Console.WriteLine("Local Host:");
            var localHostName = Dns.GetHostName();
            Console.WriteLine("\tHost Name: " + localHostName);

            PrintHostInfo(localHostName);
        }
        catch (Exception)
        {
            Console.WriteLine("Unable to resolve local host\n");
        }

        // Get and print info for hosts given on command line
        foreach (var arg in args)
        {
            Console.WriteLine(arg + ":");
            PrintHostInfo(arg);
        }
    }

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
        catch (Exception)
        {
            Console.WriteLine("\tUnable to resolve host: " + host + "\n");
        }
    }
}