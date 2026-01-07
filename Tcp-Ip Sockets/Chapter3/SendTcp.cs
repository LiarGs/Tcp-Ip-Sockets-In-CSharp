using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter3;

internal static class SendTcp
{
    public static void Example(string[] args)
    {
        if (args.Length != 2) // Test for correct # of args
            throw new ArgumentException("Parameters: <Destination> <Port>");

        var server   = args[0];            // Destination address
        var servPort = int.Parse(args[1]); // Destination port

        // Create socket that is connected to server on specified port
        var client    = new TcpClient(server, servPort);
        var netStream = client.GetStream();

        var quote = new ItemQuote(1234567890987654L, "5mm Super Widgets",
            1000, 12999, true, false);

        // Send text-encoded quote
        var coder      = new ItemQuoteEncoderText();
        var codedQuote = coder.Encode(quote);
        Console.WriteLine("Sending Text-Encoded Quote (" + codedQuote.Length + " bytes): ");
        Console.WriteLine(quote);

        netStream.Write(codedQuote, 0, codedQuote.Length);

        // Receive binary-encoded quote
        var decoder       = new ItemQuoteDecoderBin();
        var receivedQuote = decoder.Decode(client.GetStream());
        Console.WriteLine("Received Binary-Encode Quote:");
        Console.WriteLine(receivedQuote);

        netStream.Close();
        client.Close();
    }
}