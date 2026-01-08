using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter4;

public interface IDispatcher
{
    void StartDispatching(TcpListener listener, ILogger logger, IProtocolFactory protoFactory);
}