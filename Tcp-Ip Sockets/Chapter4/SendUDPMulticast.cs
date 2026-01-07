using System.Net;
using System.Net.Sockets;
using TcpIpSocketsLearn.Chapter3;

namespace TcpIpSocketsLearn.Chapter4;

public class SendUdpMulticast
{
    public static void Example(string[] args)
    {
        if (args.Length < 2 || args.Length > 3) // Test for correct # of args
            throw new ArgumentException("Parameter(s): <Multicast Addr> <Port> [<TTL>]");

        var destAddr = IPAddress.Parse(args[0]); // Destination address

        if (!MCIPAddress.IsValid(args[0]))
            throw new ArgumentException("Valid MC addr: 224.0.0.0 - 239.255.255.255");

        var destPort = int.Parse(args[1]); // Destination port

        // Time to live for datagram
        var ttl = args.Length == 3 ? int.Parse(args[2]) : 1; // Default TTL

        var quote = new ItemQuote(1234567890987654L, "5mm Super Widgets",
            1000, 12999, true, false);

        // Multicast socket to sending
        var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Set the Time to Live
        sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, ttl);

        var encoder    = new ItemQuoteEncoderText(); // Text encoding
        var codedQuote = encoder.Encode(quote);

        // Create an IP endpoint class instance
        var ipep = new IPEndPoint(destAddr, destPort);

        // Create and send a packet
        sock.SendTo(codedQuote, 0, codedQuote.Length, SocketFlags.None, ipep);

        sock.Close();
    }
}