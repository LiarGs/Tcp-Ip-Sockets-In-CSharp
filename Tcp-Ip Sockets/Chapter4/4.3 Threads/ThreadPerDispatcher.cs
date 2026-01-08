using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter4;

internal class ThreadPerDispatcher : IDispatcher
{
    public void StartDispatching(TcpListener listener, ILogger logger, IProtocolFactory protoFactory)
    {
        // Run forever, accepting and spawning threads to service each connection
        for (;;)
        {
            try
            {
                listener.Start();
                var clientSocket = listener.AcceptSocket(); // Block waiting for connection
                var protocol     = protoFactory.CreateProtocol(clientSocket, logger);
                var thread       = new Thread(protocol.HandleClient);
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