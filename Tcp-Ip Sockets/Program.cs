using TcpIpSocketsLearn.Chapter2;
using TcpIpSocketsLearn.Chapter3;
using TcpIpSocketsLearn.Chapter4;

namespace TcpIpSocketsLearn;

internal abstract class Program
{
    private static void Main(string[] args)
    {
        string[] testArgs = ["localhost", "TestEcho"];
        TcpEchoClient.Example(testArgs);
    }
}