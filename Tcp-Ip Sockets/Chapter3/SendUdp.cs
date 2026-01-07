using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter3;

internal static class SendUdp
{
    public static void Example(string[] args)
    {
        if (args.Length != 2 && args.Length != 3) // Test for correct # of args
            throw new ArgumentException("Parameter(s): <Destination> <Port> [<encoding>]");

        var server   = args[0];            // Server name or IP address
        var destPort = int.Parse(args[1]); // Destination port

        var quote = new ItemQuote(1234567890987654L, "5mm Super Widgets",
            1000, 12999, true, false);

        var client = new UdpClient(); // UDP socket for sending

        var encoder = args.Length == 3 ? new ItemQuoteEncoderText(args[2]) : new ItemQuoteEncoderText();

        var codedQuote = encoder.Encode(quote);

        client.Send(codedQuote, codedQuote.Length, server, destPort);

        client.Close();
    }
}