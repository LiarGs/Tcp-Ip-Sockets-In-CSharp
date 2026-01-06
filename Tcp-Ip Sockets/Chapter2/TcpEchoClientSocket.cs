using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpIpSocketsLearn.Chapter2;

internal static class TcpEchoClientSocket
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

        Socket? sock = null;

        try
        {
            // Create a TCP socket instance
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var ipV4Address = Dns.GetHostEntry(server).AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            // Creates server IPEndPoint instance. We assume Resolve return at least one address
            var serverEndPoint = new IPEndPoint(ipV4Address, servPort);
            // Connect the socket to server on specified port
            sock.Connect(serverEndPoint);
            Console.WriteLine("Connected to server... sending echo string");

            // Send the encoded string to the server
            sock.Send(byteBuffer, 0, byteBuffer.Length, SocketFlags.None);

            Console.WriteLine("Sent {0} bytes to server...", byteBuffer.Length);

            var totalBytesRcvd = 0; // Total bytes received so far

            // Receive the same string back from the server
            while (totalBytesRcvd < byteBuffer.Length)
            {
                // Bytes received in last read
                var bytesRcvd = sock.Receive(byteBuffer, totalBytesRcvd,
                    byteBuffer.Length - totalBytesRcvd, SocketFlags.None);
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
            sock?.Close();
        }
    }
}