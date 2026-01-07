using System.Net;
using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter4;

internal class TcpEchoServerThread
{
    public static void Example(string[] args)
    {
        if (args.Length != 1) // Test for correct # of args
            throw new ArgumentException("Parameter(s): <Port>");

        var echoServPort = int.Parse(args[0]); // Server port

        // Create a TcpListener socket to accept client connection requests
        var listener = new TcpListener(IPAddress.Any, echoServPort);

        ILogger logger = new ConsoleLogger(); // Log messages to console

        listener.Start();

        // Run forever, accepting and spawning threads to service each connection
        for (;;)
        {
            try
            {
                var clientSocket = listener.AcceptSocket(); // Block waiting for connection
                var protocol     = new EchoProtocol(clientSocket, logger);
                var thread       = new Thread(protocol.Handleclient);
                thread.Start();
                logger.WriteEntry("Created and started Thread = " + thread.GetHashCode());
            }
            catch (IOException e)
            {
                logger.WriteEntry("Exception = " + e.Message);
            }
        }
        /* NOT REACHED */
    }
}