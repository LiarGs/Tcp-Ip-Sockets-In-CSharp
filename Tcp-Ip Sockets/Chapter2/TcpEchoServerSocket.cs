using System.Net;
using System.Net.Sockets;

namespace Tcp_Ip_Sockets.Chapter2;

internal static class TcpEchoServerSocket
{
    private const int BUFSIZE = 32; // Size of receive buffer
    private const int BACKLOG = 5;  // Outstanding connection queue max size

    public static void Example(string[] args)
    {
        if (args.Length > 1) // Test for correct # of args
            throw new ArgumentException("Parameters: [<Port>]");

        var servPort = args.Length == 1 ? int.Parse(args[0]) : 7;

        Socket? server = null;

        try
        {
            // Create a socket to accept client connections
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            server.Bind(new IPEndPoint(IPAddress.Any, servPort));

            server.Listen(BACKLOG);
        }
        catch (SocketException se)
        {
            Console.WriteLine(se.ErrorCode + ": " + se.Message);
            Environment.Exit(se.ErrorCode);
        }

        var rcvBuffer = new byte[BUFSIZE]; // Receive buffer
        int bytesRcvd;                     // Received byte count

        for (;;) // Run forever, accepting and servicing connections
        {
            Socket? client = null;

            try
            {
                client = server.Accept(); // Get client connection

                Console.Write("Handling client at " + client.RemoteEndPoint + " - ");

                // Receive until client closes connection, indicated by 0 return value
                var totalBytesEchoed = 0;
                while ((bytesRcvd = client.Receive(rcvBuffer, 0, rcvBuffer.Length, SocketFlags.None)) > 0)
                {
                    client.Send(rcvBuffer, 0, bytesRcvd, SocketFlags.None);
                    totalBytesEchoed += bytesRcvd;
                }

                Console.WriteLine("echoed {0} bytes.", totalBytesEchoed);

                client.Close(); // Close the socket. We are done with this client!
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                client?.Close();
            }
        }
    }
}