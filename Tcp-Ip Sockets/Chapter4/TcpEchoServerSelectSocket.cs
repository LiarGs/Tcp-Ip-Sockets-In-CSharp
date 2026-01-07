using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter4;

internal static class TcpEchoServerSelectSocket
{
    private const int BUFSIZE          = 32;   // Size of receive buffer
    private const int BACKLOG          = 5;    // Outstanding conn queue max size
    private const int SERVER1_PORT     = 8080; // Port for second echo server
    private const int SERVER2_PORT     = 8081; // Port for second echo server
    private const int SERVER3_PORT     = 8082; // Port for third echo server
    private const int SELECT_WAIT_TIME = 1000; // Microsecs for Select() to wait

    public static void Example(string[] args)
    {
        Socket? server1 = null;
        Socket? server2 = null;
        Socket? server3 = null;

        try
        {
            // Create a socket to accept client connections
            server1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server3 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            server1.Bind(new IPEndPoint(IPAddress.Any, SERVER1_PORT));
            server2.Bind(new IPEndPoint(IPAddress.Any, SERVER2_PORT));
            server3.Bind(new IPEndPoint(IPAddress.Any, SERVER3_PORT));

            server1.Listen(BACKLOG);
            server2.Listen(BACKLOG);
            server3.Listen(BACKLOG);
        }
        catch (SocketException se)
        {
            Console.WriteLine(se.ErrorCode + ": " + se.Message);
            Environment.Exit(se.ErrorCode);
        }

        var rcvBuffer = new byte[BUFSIZE]; // Receive buffer
        int bytesRcvd;                     // Received byte count

        for (;;)
        {
            // Run forever, accepting and servicing connections

            Socket? client = null;

            // Create an array list of all three sockets
            var acceptList = new ArrayList
            {
                server1,
                server2,
                server3
            };

            try
            {
                // The Select call will check readable status of each socket in the list
                Socket.Select(acceptList, null, null, SELECT_WAIT_TIME);

                // The acceptList will now contain ONLY the server sockets with pending connections:
                foreach (var server in acceptList)
                {
                    client = ((Socket)server).Accept(); // Get client connection

                    var localEndPoint = (IPEndPoint)((Socket)server).LocalEndPoint;
                    Console.Write("Server port "           + localEndPoint.Port);
                    Console.Write(" - handling client at " + client.RemoteEndPoint + " - ");

                    // Receive until client closes connection, indicated by 0 return value
                    int totalBytesEchoed = 0;
                    while ((bytesRcvd = client.Receive(rcvBuffer, 0, rcvBuffer.Length, SocketFlags.None)) > 0)
                    {
                        client.Send(rcvBuffer, 0, bytesRcvd, SocketFlags.None);
                        totalBytesEchoed += bytesRcvd;
                    }

                    Console.WriteLine("echoed {0} bytes.", totalBytesEchoed);

                    client.Close(); // Close the socket. We are done with this client!
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                client?.Close();
            }
        }
    }
}