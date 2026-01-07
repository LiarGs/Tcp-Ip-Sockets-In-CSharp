using System.Net;
using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter4;

internal static class TcpEchoServerAsync
{
    private class ClientState
    {
        // Object to contain client state, including the client socket and the reception buffer

        private const int    BUFSIZE = 32; // Size of receive buffer
        private       byte[] _RcvBuffer;
        private       Socket _ClientSocket;

        public ClientState(Socket clientSocket)
        {
            _ClientSocket = clientSocket;
            _RcvBuffer    = new byte[BUFSIZE]; // Receive buffer
        }

        public byte[] RcvBuffer => _RcvBuffer;

        public Socket ClientSocket => _ClientSocket;
    }

    private const int BACKLOG = 5; // Outstanding connection queue max size

    public static void Example(string[] args)
    {
        if (args.Length != 1) // Test for correct # of args
            throw new ArgumentException("Parameters: <Port>");

        var servPort = int.Parse(args[0]);

        // Create a Socket to accept client connections
        var servSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        servSock.Bind(new IPEndPoint(IPAddress.Any, servPort));
        servSock.Listen(BACKLOG);

        for (;;)
        {
            // Run forever, accepting and servicing connections
            Console.WriteLine("Thread {0} ({1}) - Main(): calling BeginAccept()",
                Thread.CurrentThread.GetHashCode(),
                Thread.CurrentThread.ThreadState);

            var result = servSock.BeginAccept(_AcceptCallback, servSock);
            _DoOtherStuff();

            // Wait for the EndAccept before issuing a new BeginAccept
            result.AsyncWaitHandle.WaitOne();
        }
    }

    private static void _AcceptCallback(IAsyncResult asyncResult)
    {
        var     servSock     = (Socket)asyncResult.AsyncState;
        Socket? clientSocket = null;

        try
        {
            clientSocket = servSock.EndAccept(asyncResult);

            Console.WriteLine("Thread {0} ({1}) - AcceptCallback(): handling client at {2}",
                Thread.CurrentThread.GetHashCode(),
                Thread.CurrentThread.ThreadState,
                clientSocket.RemoteEndPoint);

            var clientState = new ClientState(clientSocket);

            clientSocket.BeginReceive(clientState.RcvBuffer, 0, clientState.RcvBuffer.Length, SocketFlags.None,
                _ReceiveCallback, clientState);
        }
        catch (SocketException se)
        {
            Console.WriteLine(se.ErrorCode + ": " + se.Message);
            clientSocket?.Close();
        }
    }

    private static void _ReceiveCallback(IAsyncResult asyncResult)
    {
        var clientState = (ClientState)asyncResult.AsyncState;

        try
        {
            int recvMsgSize = clientState.ClientSocket.EndReceive(asyncResult);

            if (recvMsgSize > 0)
            {
                Console.WriteLine("Thread {0} ({1}) - ReceiveCallback(): received {2} bytes",
                    Thread.CurrentThread.GetHashCode(),
                    Thread.CurrentThread.ThreadState,
                    recvMsgSize);

                clientState.ClientSocket.BeginSend(clientState.RcvBuffer, 0, recvMsgSize, SocketFlags.None,
                    _SendCallback, clientState);
            }
            else
            {
                clientState.ClientSocket.Close();
            }
        }
        catch (SocketException se)
        {
            Console.WriteLine(se.ErrorCode + ": " + se.Message);
            clientState?.ClientSocket.Close();
        }
    }

    private static void _SendCallback(IAsyncResult asyncResult)
    {
        var clientState = (ClientState)asyncResult.AsyncState;

        try
        {
            int bytesSent = clientState.ClientSocket.EndSend(asyncResult);

            Console.WriteLine("Thread {0} ({1}) - SendCallback(): sent {2} bytes",
                Thread.CurrentThread.GetHashCode(),
                Thread.CurrentThread.ThreadState,
                bytesSent);

            clientState.ClientSocket.BeginReceive(clientState.RcvBuffer, 0, clientState.RcvBuffer.Length,
                SocketFlags.None, _ReceiveCallback, clientState);
        }
        catch (SocketException se)
        {
            Console.WriteLine(se.ErrorCode + ": " + se.Message);
            clientState?.ClientSocket.Close();
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