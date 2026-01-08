using System.Net;
using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter4;

internal class TcpClientShutdown : TcpClient
{
    public TcpClientShutdown() : base()
    {
    }

    public TcpClientShutdown(IPEndPoint localEP) : base(localEP)
    {
    }

    public TcpClientShutdown(string server, int port) : base(server, port)
    {
    }

    public void Shutdown(SocketShutdown socketShutdown)
    {
        // Invoke the Shutdown method on the underlying socket
        Client.Shutdown(socketShutdown);
    }

    public EndPoint GetRemoteEndPoint()
    {
        // Return the RemoteEndPoint from the underlying socket
        return Client.RemoteEndPoint;
    }
}