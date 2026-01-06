using TcpIpSockets.Chapter2;

namespace TcpIpSocketsLearn;

internal abstract class Program
{
    private static void Main(string[] args)
    {
        string[] testArgs = ["localhost", "EchoTest"];
        UdpMaxSizeTester.Example();
    }
}