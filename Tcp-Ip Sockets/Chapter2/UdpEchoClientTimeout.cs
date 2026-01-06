using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Tcp_Ip_Sockets.Chapter2;

internal static class UdpEchoClientTimeout
{
    private const int TIMEOUT  = 3000; // Resend timeout (milliseconds)
    private const int MAXTRIES = 5;    // Maximum retransmissions

    public static void Example(string[] args)
    {
        if (args.Length < 2 || args.Length > 3) // Test for correct # of args
            throw new ArgumentException("Parameters: <Server> <Word> [<Port>]");

        var server = args[0]; // Server name or IP address

        // Use port argument if supplied, otherwise default to 7
        var servPort = args.Length == 3 ? int.Parse(args[2]) : 7;

        // Create socket that is connected to server on specified port
        var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Set the reception timeout for this socket
        sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, TIMEOUT);

        var ipV4Address = Dns.GetHostEntry(server).AddressList
            .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

        var remoteIPEndPoint = new IPEndPoint(ipV4Address, servPort);
        var remoteEndPoint   = (EndPoint)remoteIPEndPoint;

        // Convert input String to a packet of bytes
        var sendPacket = Encoding.ASCII.GetBytes(args[1]);
        var rcvPacket  = new byte[sendPacket.Length];

        var tries            = 0; // Packets may be lost, so we have to keep trying
        var receivedResponse = false;

        do
        {
            sock.SendTo(sendPacket, remoteEndPoint); // Send the echo string

            Console.WriteLine("Sent {0} bytes to the server...", sendPacket.Length);

            try
            {
                // Attempt echo reply receive
                sock.ReceiveFrom(rcvPacket, ref remoteEndPoint);
                receivedResponse = true;
            }
            catch (SocketException se)
            {
                tries++;
                if (se.ErrorCode == 10060) // WSAETIMEDOUT: Connection timed out
                    Console.WriteLine("Timed out, {0} more tries...", MAXTRIES - tries);
                else // We encountered an error other than a timeout, output error message
                    Console.WriteLine(se.ErrorCode + ": " + se.Message);
            }
        } while (!receivedResponse && tries < MAXTRIES);

        if (receivedResponse)
            Console.WriteLine("Received {0} bytes from {1}: {2}",
                rcvPacket.Length, remoteEndPoint,
                Encoding.ASCII.GetString(rcvPacket, 0, rcvPacket.Length));
        else
            Console.WriteLine("No response - giving up.");

        sock.Close();
    }
}