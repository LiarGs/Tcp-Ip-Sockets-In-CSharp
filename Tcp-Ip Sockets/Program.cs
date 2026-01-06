using Tcp_Ip_Sockets.Chapter2;

namespace Tcp_Ip_Sockets;

internal abstract class Program
{
    private static void Main(string[] args)
    {
        string[] testArgs = ["localhost", "EchoTest"];
        UdpEchoClientTimeout.Example(testArgs);
    }
}