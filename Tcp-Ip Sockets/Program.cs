using TcpIpSocketsLearn.Chapter4;

namespace TcpIpSocketsLearn;

internal abstract class Program
{
    private static void Main(string[] args)
    {
        string[] testArgs = ["localhost", "TestEcho"];
        TranscodeClient.Example(testArgs);
    }
}