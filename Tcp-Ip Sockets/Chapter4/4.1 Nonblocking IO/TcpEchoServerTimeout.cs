using System.Net;
using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter4;

internal static class TcpEchoServerTimeout
{
    private const int BUFSIZE   = 32;    // Size of receive buffer
    private const int BACKLOG   = 5;     // Outstanding conn queue max size
    private const int TIMELIMIT = 10000; // Default time limit (ms)

    private static void Example(string[] args)
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
        var totalBytesEchoed = 0;          // Total bytes sent

        for (;;)
        {
            // Run forever, accepting and servicing connections
            Socket? client = null;

            try
            {
                client = server.Accept(); // Get client connection

                var startTime = DateTime.Now;

                // Set the ReceiveTimeout
                client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, TIMELIMIT);

                Console.Write("Handling client at " + client.RemoteEndPoint + " - ");

                // Receive until client closes connection, indicated by 0 return value
                totalBytesEchoed = 0;
                while ((bytesRcvd = client.Receive(rcvBuffer, 0, rcvBuffer.Length, SocketFlags.None)) > 0)
                {
                    client.Send(rcvBuffer, 0, bytesRcvd, SocketFlags.None);
                    totalBytesEchoed += bytesRcvd;

                    // Check elapsed time
                    var elapsed = DateTime.Now - startTime;
                    if (TIMELIMIT - elapsed.TotalMilliseconds < 0)
                    {
                        Console.WriteLine("Aborting client, timeLimit " + TIMELIMIT        +
                                          "ms exceeded; echoed "        + totalBytesEchoed + " bytes");
                        client.Close();
                        throw new SocketException(10060);
                    }

                    // Set the ReceiveTimeout
                    client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout,
                        (int)(TIMELIMIT - elapsed.TotalMilliseconds));
                }

                Console.WriteLine("echoed {0} bytes.", totalBytesEchoed);

                client.Close(); // Close the socket. We are done with this client!
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10060) // WSAETIMEDOUT: Connection timed out
                {
                    Console.WriteLine("Aborting client, timeLimit " + TIMELIMIT        +
                                      "ms exceeded; echoed "        + totalBytesEchoed + " bytes");
                }
                else
                {
                    Console.WriteLine(se.ErrorCode + ": " + se.Message);
                }

                client?.Close();
            }
        }
    }
}