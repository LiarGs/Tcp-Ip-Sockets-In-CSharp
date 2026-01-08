using System.Net;
using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter2;

internal static class UdpEchoServer
{
    public static void Example(string[] args)
    {
        if (args.Length > 1) throw new ArgumentException("Parameters: <Port>");

        var servPort = args.Length == 1 ? int.Parse(args[0]) : 7;

        UdpClient? client = null;

        try
        {
            client = new UdpClient(servPort);
        }
        catch (SocketException se)
        {
            Console.WriteLine(se.ErrorCode + ": " + se.Message);
            Environment.Exit(se.ErrorCode);
        }

        var remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);

        for (;;)
            try
            {
                var byteBuffer = client.Receive(ref remoteIPEndPoint);
                Console.Write("Handling client at " + remoteIPEndPoint + " - ");

                client.Send(byteBuffer, byteBuffer.Length, remoteIPEndPoint);
                Console.WriteLine("echoed {0} bytes.", byteBuffer.Length);
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.ErrorCode + ": " + se.Message);
            }
    }
}