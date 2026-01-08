using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter4;

public class IterativeDispatcher : IDispatcher
{
    public void StartDispatching(TcpListener listener, ILogger logger, IProtocolFactory protoFactory)
    {
        logger.WriteEntry("IterativeDispatcher starting...");

        for (;;)
        {
            try
            {
                listener.Start();
                var clientSocket = listener.AcceptSocket();
                var protocol     = protoFactory.CreateProtocol(clientSocket, logger);
                protocol.HandleClient();
            }
            catch (Exception e)
            {
                logger.WriteEntry("Exception occurred: " + e.Message);
            }
        }
    }
}