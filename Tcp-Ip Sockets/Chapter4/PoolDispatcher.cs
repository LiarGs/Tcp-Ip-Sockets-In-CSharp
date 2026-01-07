using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter4;

internal class PoolDispatcher : IDispatcher
{
    private const int NUMTHREADS = 8; // Default thread pool size

    private int _NumThreads; // Number of threads in pool

    public PoolDispatcher()
    {
        _NumThreads = NUMTHREADS;
    }

    public PoolDispatcher(int numThreads)
    {
        _NumThreads = numThreads;
    }

    public void StartDispatching(TcpListener listener, ILogger logger, IProtocolFactory protoFactory)
    {
        // Create N threads, each running an iterative server
        for (int i = 0; i < _NumThreads; i++)
        {
            var dl     = new DispatchLoop(listener, logger, protoFactory);
            var thread = new Thread(dl.RunDispatcher);
            thread.Start();
            logger.WriteEntry("Created and started Thread = " + thread.GetHashCode());
        }
    }
}

internal class DispatchLoop
{
    private TcpListener      _Listener;
    private ILogger          _Logger;
    private IProtocolFactory _ProtoFactory;

    public DispatchLoop(TcpListener listener, ILogger logger, IProtocolFactory protoFactory)
    {
        _Listener     = listener;
        _Logger       = logger;
        _ProtoFactory = protoFactory;
    }

    public void RunDispatcher()
    {
        // Run forever, accepting and handling each connection
        for (;;)
        {
            try
            {
                var clientSocket = _Listener.AcceptSocket(); // Block waiting for connection
                var protocol     = _ProtoFactory.CreateProtocol(clientSocket, _Logger);
                protocol.Handleclient();
            }
            catch (SocketException se)
            {
                _Logger.WriteEntry("Exception = " + se.Message);
            }
        }
    }
}