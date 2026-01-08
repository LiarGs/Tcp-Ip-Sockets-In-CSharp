using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter4;

public class EchoProtocolFactory : IProtocolFactory
{
    public IProtocol CreateProtocol(Socket clientSock, ILogger logger)
    {
        return new EchoProtocol(clientSock, logger);
    }
}