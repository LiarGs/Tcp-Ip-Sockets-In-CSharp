using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Tcp_Ip_Sockets.Chapter2;

internal static class UdpEchoClient
{
    public static void Example(string[] args)
    {
        if (args.Length < 2 || args.Length > 3) // Test for correct # of args
            throw new ArgumentException("Parameters: <Server> <Word> [<Port>]");

        var server = args[0]; // Server name or IP address

        // Use port argument if supplied, otherwise default to 7
        var servPort = args.Length == 3 ? int.Parse(args[2]) : 7;

        // Convert input String to an array of bytes
        var sendPacket = Encoding.ASCII.GetBytes(args[1]);

        // Create a UdpClient instance
        var client = new UdpClient();

        try
        {
            // Send the echo string to the specified host and port
            client.Send(sendPacket, sendPacket.Length, server, servPort);

            Console.WriteLine("Sent {0} bytes to the server...", sendPacket.Length);

            // This IPEndPoint instance will be populated with the remote sender’s
            // endpoint information after the Receive() call
            var remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);

            // Attempt echo reply receive
            var rcvPacket = client.Receive(ref remoteIPEndPoint);

            Console.WriteLine("Received {0} bytes from {1}: {2}",
                rcvPacket.Length, remoteIPEndPoint,
                Encoding.ASCII.GetString(rcvPacket, 0, rcvPacket.Length));
        }
        catch (SocketException se)
        {
            Console.WriteLine(se.ErrorCode + ": " + se.Message);
        }

        client.Close();
    }
}