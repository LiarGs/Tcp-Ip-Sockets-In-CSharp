using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpIpSocketsLearn.Chapter4;

public static class TranscodeServer
{
    private const int _BUFSIZE = 1024; // Size of read buffer

    public static void Example(string[] args)
    {
        if (args.Length != 1) // Test for correct # of args
            throw new ArgumentException("Parameter(s): <Port>");

        var servPort = int.Parse(args[0]); // Server port

        // Create a TcpListener to accept client connection requests
        var listener = new TcpListener(IPAddress.Any, servPort);
        listener.Start();

        var buffer = new byte[_BUFSIZE]; // Allocate read/write buffer
        int bytesRead;                   // Number of bytes read
        for (;;)
        {
            // Run forever, accepting and servicing connections
            // Wait for client to connect, then create a new TcpClient
            var client = listener.AcceptTcpClient();

            Console.WriteLine("\nHandling client...");

            // Get the input and output streams from socket
            var netStream = client.GetStream();

            int totalBytesRead    = 0;
            int totalBytesWritten = 0;

            var uniDecoder = Encoding.Unicode.GetDecoder();

            // Receive until client closes connection, indicated by 0 return
            while ((bytesRead = netStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                totalBytesRead += bytesRead;

                // Convert the incoming bytes to Unicode char array
                int charCount         = uniDecoder.GetCharCount(buffer, 0, bytesRead);
                var chars             = new char[charCount];
                int charsDecodedCount = uniDecoder.GetChars(buffer, 0, bytesRead, chars, 0);

                // Convert the Unicode char array to UTF8 bytes
                int byteCount    = Encoding.UTF8.GetByteCount(chars, 0, charsDecodedCount);
                var outputBuffer = new byte[byteCount];
                Encoding.UTF8.GetBytes(chars, 0, charsDecodedCount, outputBuffer, 0);

                // Send UTF8 bytes back to client
                netStream.Write(outputBuffer, 0, outputBuffer.Length);
                totalBytesWritten += outputBuffer.Length;
            }

            Console.WriteLine("Total bytes read: {0}",    totalBytesRead);
            Console.WriteLine("Total bytes written: {0}", totalBytesWritten);
            Console.WriteLine("Closing client connection...");

            netStream.Close(); // Close the stream
            client.Close();    // Close the socket
        }
        /* NOT REACHED */
    }
}