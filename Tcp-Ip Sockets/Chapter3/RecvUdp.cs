using System.Net;
using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter3;

internal static class RecvUdp
{
    public static void Example(string[] args)
    {
        if (args.Length != 1 && args.Length != 2) // Test for correct # of args
            throw new ArgumentException("Parameter(s): <Port> [<encoding>]");

        var port = int.Parse(args[0]); // Receiving Port

        var client = new UdpClient(port); // UDP socket for receiving

        var packet           = new byte[ItemQuoteTextConst.MAX_WIRE_LENGTH];
        var remoteIPEndPoint = new IPEndPoint(IPAddress.Any, port);

        packet = client.Receive(ref remoteIPEndPoint);

        // Which encoding 
        var decoder = args.Length == 2
            ? new ItemQuoteDecoderText(args[1])
            : new ItemQuoteDecoderText();

        var quote = decoder.Decode(packet);
        Console.WriteLine(quote);

        client.Close();
    }
}