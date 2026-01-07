using System.Net;
using System.Net.Sockets;
using TcpIpSocketsLearn.Chapter3;

namespace TcpIpSocketsLearn.Chapter4;

public class RecvUdpMulticast
{
    public static void Example(string[] args)
    {
        if (args.Length != 2) // Test for correct # of args
            throw new ArgumentException("Parameter(s): <Multicast Addr> <Port>");

        var address = IPAddress.Parse(args[0]); // Multicast address

        if (!MCIPAddress.IsValid(args[0]))
            throw new ArgumentException("Valid MC addr: 224.0.0.0 - 239.255.255.255");

        var port = int.Parse(args[1]); // Multicast port

        // Multicast receiving socket
        var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Set the reuse address option
        sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

        // Create an IPEndPoint and bind to it
        var ipep = new IPEndPoint(IPAddress.Any, port);
        sock.Bind(ipep);

        // Add membership in the multicast group
        sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
            new MulticastOption(address, IPAddress.Any));

        var receivePoint     = new IPEndPoint(IPAddress.Any, 0);
        var tempReceivePoint = (EndPoint)receivePoint;

        // Create and receive a datagram
        var packet = new byte[ItemQuoteTextConst.MAX_WIRE_LENGTH];
        var length = sock.ReceiveFrom(packet, 0, ItemQuoteTextConst.MAX_WIRE_LENGTH,
            SocketFlags.None, ref tempReceivePoint);

        var decoder = new ItemQuoteDecoderText(); // Text decoding
        var quote   = decoder.Decode(packet);
        Console.WriteLine(quote);

        // Drop membership in the multicast group
        sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership,
            new MulticastOption(address, IPAddress.Any));
        sock.Close();
    }
}