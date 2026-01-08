using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter4;

public class RandomProtocolFactory : IProtocolFactory
{
    public IProtocol CreateProtocol(Socket clientSock, ILogger logger)
    {
        return new RandomProtocol(clientSock, logger);
    }
}