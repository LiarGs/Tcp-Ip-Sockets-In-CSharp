using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter4;

public interface IProtocolFactory
{
    IProtocol CreateProtocol(Socket clientSock, ILogger logger);
}