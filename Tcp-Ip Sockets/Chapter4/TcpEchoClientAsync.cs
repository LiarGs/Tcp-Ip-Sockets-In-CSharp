using System.Net.Sockets;
using System.Text;

namespace TcpIpSocketsLearn.Chapter4;

internal static class TcpEchoClientAsync
{
    private class ClientState
    {
        // Object to contain client state, including the network stream and the send/recv buffer

        private byte[]        _ByteBuffer;
        private NetworkStream _NetStream;
        private StringBuilder _EchoResponse;

        public ClientState(NetworkStream netStream, byte[] byteBuffer)
        {
            _NetStream    = netStream;
            _ByteBuffer   = byteBuffer;
            _EchoResponse = new StringBuilder();
        }

        public NetworkStream NetStream => _NetStream;

        public byte[] ByteBuffer => _ByteBuffer;

        public string EchoResponse => _EchoResponse.ToString();

        public int TotalBytes { get; private set; } = 0;

        public void AppendResponse(string response)
        {
            _EchoResponse.Append(response);
        }

        public void AddToTotalBytes(int count)
        {
            TotalBytes += count;
        }
    }

    // A manual event signal we will trigger when all reads are complete:
    public static ManualResetEvent ReadDone = new ManualResetEvent(false);

    public static void Example(string[] args)
    {
        if (args.Length < 2 || args.Length > 3)
        {
            // Test for correct # of args
            throw new ArgumentException("Parameters: <Server> <Word> [<Port>]");
        }

        var server = args[0]; // Server name or IP address

        // Use port argument if supplied, otherwise default to 7
        var servPort = args.Length == 3 ? int.Parse(args[2]) : 7;

        Console.WriteLine("Thread {0} ({1}) - Main()",
            Thread.CurrentThread.GetHashCode(),
            Thread.CurrentThread.ThreadState);

        // Create TcpClient that is connected to server on specified port
        var client = new TcpClient();
        client.Connect(server, servPort);

        Console.WriteLine("Thread {0} ({1}) - Main(): connected to server",
            Thread.CurrentThread.GetHashCode(),
            Thread.CurrentThread.ThreadState);

        var netStream   = client.GetStream();
        var clientState = new ClientState(netStream, Encoding.ASCII.GetBytes(args[1]));

        // Send the encoded string to the server
        var result = netStream.BeginWrite(clientState.ByteBuffer, 0, clientState.ByteBuffer.Length,
            _WriteCallback, clientState);

        _DoOtherStuff();

        result.AsyncWaitHandle.WaitOne(); // block until EndWrite is called

        // Receive the same string back from the server
        netStream.BeginRead(clientState.ByteBuffer, clientState.TotalBytes,
            clientState.ByteBuffer.Length - clientState.TotalBytes, _ReadCallback, clientState);

        _DoOtherStuff();

        ReadDone.WaitOne(); // Block until ReadDone is manually set

        netStream.Close(); // Close the stream
        client.Close();    // Close the socket
    }

    private static void _WriteCallback(IAsyncResult asyncResult)
    {
        var clientState = (ClientState)asyncResult.AsyncState;
        clientState?.NetStream.EndWrite(asyncResult);

        Console.WriteLine("Thread {0} ({1}) - WriteCallback(): Sent {2} bytes...",
            Thread.CurrentThread.GetHashCode(),
            Thread.CurrentThread.ThreadState, clientState?.ByteBuffer.Length);
    }

    private static void _ReadCallback(IAsyncResult asyncResult)
    {
        var clientState = (ClientState)asyncResult.AsyncState;
        int bytesRcvd   = clientState.NetStream.EndRead(asyncResult);

        clientState.AddToTotalBytes(bytesRcvd);
        clientState.AppendResponse(Encoding.ASCII.GetString(clientState.ByteBuffer, 0, bytesRcvd));

        if (clientState.TotalBytes < clientState.ByteBuffer.Length)
        {
            Console.WriteLine("Thread {0} ({1}) - ReadCallback(): Received {2} bytes...",
                Thread.CurrentThread.GetHashCode(),
                Thread.CurrentThread.ThreadState, bytesRcvd);

            clientState.NetStream.BeginRead(clientState.ByteBuffer, clientState.TotalBytes,
                clientState.ByteBuffer.Length - clientState.TotalBytes, _ReadCallback, clientState.NetStream);
        }
        else
        {
            Console.WriteLine("Thread {0} ({1}) - ReadCallback(): Received {2} total bytes: {3}",
                Thread.CurrentThread.GetHashCode(),
                Thread.CurrentThread.ThreadState,
                clientState.TotalBytes, clientState.EchoResponse);
            ReadDone.Set(); // Signal read complete event
        }
    }

    private static void _DoOtherStuff()
    {
        for (int x = 1; x <= 5; x++)
        {
            Console.WriteLine("Thread {0} ({1}) - DoOtherStuff(): {2}...",
                Thread.CurrentThread.GetHashCode(),
                Thread.CurrentThread.ThreadState, x);
            Thread.Sleep(1000);
        }
    }
}