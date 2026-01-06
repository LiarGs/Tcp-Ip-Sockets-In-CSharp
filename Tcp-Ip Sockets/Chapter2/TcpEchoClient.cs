using System.Net.Sockets;
using System.Text;

namespace TcpIpSocketsLearn.Chapter2;

internal static class TcpEchoClient
{
    public static void Example(string[] args)
    {
        if (args.Length < 2 || args.Length > 3) // Test for correct # of args
            throw new ArgumentException("Parameters: <Server> <Word> [<Port>]");

        var server = args[0]; // Server name or IP address

        // Convert input String to bytes
        var byteBuffer = Encoding.ASCII.GetBytes(args[1]);

        // Use port argument if supplied, otherwise default to 7
        var servPort = args.Length == 3 ? int.Parse(args[2]) : 7;

        TcpClient?     client    = null;
        NetworkStream? netStream = null;

        try
        {
            // Create socket that is connected to server on specified port
            client = new TcpClient(server, servPort);

            Console.WriteLine("Connected to server... sending echo string");

            netStream = client.GetStream();

            // Send the encoded string to the server
            netStream.Write(byteBuffer, 0, byteBuffer.Length);

            Console.WriteLine("Sent {0} bytes to server...", byteBuffer.Length);

            var totalBytesRcvd = 0; // Total bytes received so far

            // Receive the same string back from the server
            while (totalBytesRcvd < byteBuffer.Length)
            {
                // Bytes received in last read
                var bytesRcvd = netStream.Read(byteBuffer, totalBytesRcvd,
                    byteBuffer.Length - totalBytesRcvd);
                if (bytesRcvd == 0)
                {
                    Console.WriteLine("Connection closed prematurely.");
                    break;
                }

                totalBytesRcvd += bytesRcvd;
            }

            Console.WriteLine("Received {0} bytes from server: {1}", totalBytesRcvd,
                Encoding.ASCII.GetString(byteBuffer, 0, totalBytesRcvd));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            netStream?.Close();
            client?.Close();
        }
    }
}