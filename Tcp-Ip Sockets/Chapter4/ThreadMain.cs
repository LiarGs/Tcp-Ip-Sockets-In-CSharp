using System.Net;
using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter4;

internal static class ThreadMain
{
    public static void Example(string[] args)
    {
        if (args.Length != 3) // Test for correct # of args
            throw new ArgumentException("Parameter(s): [<Optional properties>] <Port> <Protocol> <Dispatcher>");

        var servPort       = int.Parse(args[0]); // Server Port
        var protocolName   = args[1];            // Protocol name
        var dispatcherName = args[2];            // Dispatcher name

        var listener = new TcpListener(IPAddress.Any, servPort);
        listener.Start();

        ILogger logger = new ConsoleLogger(); // Log messages to console

        var objHandle    = Activator.CreateInstance(null, protocolName + "ProtocolFactory");
        var protoFactory = (IProtocolFactory)objHandle.Unwrap();

        objHandle = Activator.CreateInstance(null, dispatcherName + "Dispatcher");
        var dispatcher = (IDispatcher)objHandle.Unwrap();

        dispatcher?.StartDispatching(listener, logger, protoFactory);
        /* NOT REACHED */
    }
}