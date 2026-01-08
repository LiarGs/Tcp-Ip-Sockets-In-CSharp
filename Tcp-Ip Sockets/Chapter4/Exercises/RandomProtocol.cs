using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter4;

internal class RandomProtocol : IProtocol
{
    private const int _INTSIZE = 4;

    private Socket  _ClientSock;
    private ILogger _Logger;

    public RandomProtocol(Socket clientSock, ILogger logger)
    {
        _ClientSock = clientSock;
        _Logger     = logger;
    }

    public void HandleClient()
    {
        var entry = new ArrayList
        {
            "Client address and port = " + _ClientSock.RemoteEndPoint,
            "Thread = "                  + Thread.CurrentThread.GetHashCode(),
            "Protocol = RandomNumber"
        };

        try
        {
            var buffer         = new byte[_INTSIZE];
            var totalBytesRead = 0;
            int bytesRead;

            while (totalBytesRead < _INTSIZE)
            {
                bytesRead = _ClientSock.Receive(buffer, totalBytesRead,
                    _INTSIZE - totalBytesRead, SocketFlags.None);

                if (bytesRead == 0)
                {
                    entry.Add("Client closed connection prematurely.");
                    _ClientSock.Close();
                    _Logger.WriteEntry(entry);
                    return;
                }

                totalBytesRead += bytesRead;
            }

            var receivedRaw = BitConverter.ToInt32(buffer, 0);
            int bound       = IPAddress.NetworkToHostOrder(receivedRaw);

            entry.Add($"Received Upper Bound B = {bound}");

            if (bound < 1)
            {
                entry.Add("Invalid bound received (< 1). Closing.");
            }
            else
            {
                var randomNumber = (int)Random.Shared.NextInt64(1, (long)bound + 1);

                entry.Add($"Generated Random Number = {randomNumber}");

                int    networkOrderResult = IPAddress.HostToNetworkOrder(randomNumber);
                byte[] responseBytes      = BitConverter.GetBytes(networkOrderResult);

                _ClientSock.Send(responseBytes);
                entry.Add("Response sent.");
            }
        }
        catch (Exception e)
        {
            entry.Add("Exception = " + e.Message);
        }
        finally
        {
            _ClientSock.Close();
            _Logger.WriteEntry(entry);
        }
    }
}