using System.Net;
using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter3;

internal static class RecvTcp
{
    public static void Example(string[] args)
    {
        if (args.Length != 1) // Test for correct # of args
            throw new ArgumentException("Parameters: <Port>");

        var port = int.Parse(args[0]);

        // Create a TCPListener to accept client connections
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        var client = listener.AcceptTcpClient(); // Get client connection

        // Receive text-encoded quote
        var decoder = new ItemQuoteDecoderText();
        var quote   = decoder.Decode(client.GetStream());
        Console.WriteLine("Received Text-Encoded Quote:");
        Console.WriteLine(quote);

        // Repeat quote with binary-encoding, adding 10 cents to the price
        var encoder = new ItemQuoteEncoderBin();
        quote.UnitPrice += 10; // Add 10 cents to unit price
        Console.WriteLine("Sending (binary)...");
        var bytesToSend = encoder.Encode(quote);
        client.GetStream().Write(bytesToSend, 0, bytesToSend.Length);

        client.Close();
        listener.Stop();
    }
}