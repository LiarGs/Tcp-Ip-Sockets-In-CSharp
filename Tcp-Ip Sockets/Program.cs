using TcpIpSocketsLearn.Chapter3;

namespace TcpIpSocketsLearn;

internal abstract class Program
{
    private static void Main(string[] args)
    {
        string[] testArgs = ["localhost", "7"];
        SendUdp.Example(testArgs);
    }
}