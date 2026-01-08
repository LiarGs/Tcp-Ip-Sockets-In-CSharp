using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpIpSocketsLearn.Chapter4;

internal static class TcpEchoClientAsyncNew
{
    private class ConnectState(TcpClient client, int port)
    {
        public TcpClient Client { get; } = client;
        public int       Port   { get; } = port;
    }

    private class ClientState
    {
        public byte[]        ByteBuffer   { get; }
        public NetworkStream NetStream    { get; }
        public StringBuilder EchoResponse { get; }              = new StringBuilder();
        public int           TotalBytes   { get; private set; } = 0;

        public ClientState(NetworkStream netStream, byte[] byteBuffer)
        {
            NetStream  = netStream;
            ByteBuffer = byteBuffer;
        }

        public void AppendResponse(string response) => EchoResponse.Append(response);
        public void AddToTotalBytes(int   count)    => TotalBytes += count;
    }

    public static ManualResetEvent ConnectDone = new ManualResetEvent(false);
    public static ManualResetEvent ReadDone    = new ManualResetEvent(false);

    public static void Example(string[] args)
    {
        if (args.Length < 2 || args.Length > 3)
            throw new ArgumentException("Parameters: <Server> <Word> [<Port>]");

        string server   = args[0];
        string word     = args[1];
        int    servPort = args.Length == 3 ? int.Parse(args[2]) : 7;

        Console.WriteLine("Thread {0} - Main(): Starting...", Thread.CurrentThread.GetHashCode());

        var client = new TcpClient();

        Console.WriteLine("Thread {0} - Main(): Resolving DNS...", Thread.CurrentThread.GetHashCode());

        var connectState = new ConnectState(client, servPort);

        Dns.BeginGetHostEntry(server, _DnsCallback, connectState);

        _DoOtherStuff("Connecting");
        ConnectDone.WaitOne();

        Console.WriteLine("Thread {0} - Main(): Connected! Sending data...", Thread.CurrentThread.GetHashCode());

        NetworkStream netStream   = client.GetStream();
        var           clientState = new ClientState(netStream, Encoding.ASCII.GetBytes(word));

        IAsyncResult writeResult = netStream.BeginWrite(clientState.ByteBuffer, 0,
            clientState.ByteBuffer.Length, _WriteCallback, clientState);

        _DoOtherStuff("Sending");
        writeResult.AsyncWaitHandle.WaitOne();

        netStream.BeginRead(clientState.ByteBuffer, clientState.TotalBytes,
            clientState.ByteBuffer.Length - clientState.TotalBytes, _ReadCallback, clientState);

        _DoOtherStuff("Receiving");
        ReadDone.WaitOne();

        Console.WriteLine("Done. Closing.");
        netStream.Close();
        client.Close();
    }

    private static void _DnsCallback(IAsyncResult asyncResult)
    {
        try
        {
            IPHostEntry  hostEntry = Dns.EndGetHostEntry(asyncResult);
            ConnectState state     = (ConnectState)asyncResult.AsyncState!;

            Console.WriteLine("Thread {0} - DnsCallback(): Resolved {1} addresses.",
                Thread.CurrentThread.GetHashCode(), hostEntry.AddressList.Length);

            IPAddress? ipv4Address = hostEntry.AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            if (ipv4Address == null)
            {
                Console.WriteLine("No IPv4 address found!");
                return;
            }

            Console.WriteLine("Thread {0} - DnsCallback(): Connecting to {1}...",
                Thread.CurrentThread.GetHashCode(), ipv4Address);

            state.Client.BeginConnect(ipv4Address, state.Port, _ConnectCallback, state.Client);
        }
        catch (Exception e)
        {
            Console.WriteLine("DNS Error: " + e.Message);
            ConnectDone.Set();
        }
    }

    private static void _ConnectCallback(IAsyncResult asyncResult)
    {
        try
        {
            TcpClient client = (TcpClient)asyncResult.AsyncState!;

            client.EndConnect(asyncResult);

            Console.WriteLine("Thread {0} - ConnectCallback(): Connection established.",
                Thread.CurrentThread.GetHashCode());
        }
        catch (Exception e)
        {
            Console.WriteLine("Connect Error: " + e.Message);
        }
        finally
        {
            ConnectDone.Set();
        }
    }

    private static void _WriteCallback(IAsyncResult asyncResult)
    {
        var clientState = (ClientState)asyncResult.AsyncState!;
        clientState.NetStream.EndWrite(asyncResult);
        Console.WriteLine("Write complete.");
    }

    private static void _ReadCallback(IAsyncResult asyncResult)
    {
        var clientState = (ClientState)asyncResult.AsyncState!;
        int bytesRcvd   = clientState.NetStream.EndRead(asyncResult);

        if (bytesRcvd > 0)
        {
            clientState.AddToTotalBytes(bytesRcvd);
            clientState.AppendResponse(Encoding.ASCII.GetString(clientState.ByteBuffer, 0, bytesRcvd));

            if (clientState.TotalBytes < clientState.ByteBuffer.Length)
            {
                clientState.NetStream.BeginRead(clientState.ByteBuffer, clientState.TotalBytes,
                    clientState.ByteBuffer.Length - clientState.TotalBytes, _ReadCallback, clientState);
                return;
            }
        }

        Console.WriteLine("Received: {0}", clientState.EchoResponse);
        ReadDone.Set();
    }

    private static void _DoOtherStuff(string context)
    {
        Console.Write($"Thread {Thread.CurrentThread.GetHashCode()} - {context}: ");
        for (int i = 0; i < 3; i++)
        {
            Console.Write(".");
            Thread.Sleep(200);
        }

        Console.WriteLine();
    }
}