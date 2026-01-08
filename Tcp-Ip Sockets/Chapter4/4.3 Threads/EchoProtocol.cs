using System.Collections;
using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter4;

internal class EchoProtocol : IProtocol
{
    public const int BUFSIZE = 32; // Byte size of IO buffer

    private Socket  _ClientSock; // Connection socket
    private ILogger _Logger;     // Logging facility

    public EchoProtocol(Socket clientSock, ILogger logger)
    {
        _ClientSock = clientSock;
        _Logger     = logger;
    }

    public void HandleClient()
    {
        var entry = new ArrayList
        {
            "Client address and port = " + _ClientSock.RemoteEndPoint,
            "Thread = "                  + Thread.CurrentThread.GetHashCode()
        };

        try
        {
            // Receive until client closes connection, indicated by a SocketException
            int    recvMsgSize;                          // Size of received message
            int    totalBytesEchoed = 0;                 // Bytes received from client
            byte[] rcvBuffer        = new byte[BUFSIZE]; // Receive buffer

            // Receive until client closes connection, indicated by 0 return code
            try
            {
                while ((recvMsgSize = _ClientSock.Receive(rcvBuffer, 0, rcvBuffer.Length, SocketFlags.None)) > 0)
                {
                    _ClientSock.Send(rcvBuffer, 0, recvMsgSize, SocketFlags.None);
                    totalBytesEchoed += recvMsgSize;
                }
            }
            catch (SocketException se)
            {
                entry.Add(se.ErrorCode + ": " + se.Message);
            }

            entry.Add("Client finished; echoed " + totalBytesEchoed + " bytes.");
        }
        catch (SocketException se)
        {
            entry.Add(se.ErrorCode + ": " + se.Message);
        }

        _ClientSock.Close();
        _Logger.WriteEntry(entry);
    }
}