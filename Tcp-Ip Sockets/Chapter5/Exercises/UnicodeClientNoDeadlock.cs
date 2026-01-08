using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter5;

public static class UnicodeClientNoDeadlock
{
    private const int _BUFSIZE = 256;

    private static Socket     _ClientSock;
    private static FileStream _FileIn;
    private static byte[]     _SendBuffer;

    public static void Example(string[] args)
    {
        if (args.Length != 3)
            throw new ArgumentException("Parameter(s): <Server> <Port> <File>");

        var server   = args[0];
        var port     = int.Parse(args[1]);
        var filename = args[2];

        try
        {
            _FileIn = new FileStream(filename, FileMode.Open, FileAccess.Read);
            var fileOut = new FileStream(filename + ".utf8", FileMode.Create);

            using var client = new TcpClient(server, port);
            _ClientSock = client.Client;

            _SendBuffer = new byte[_BUFSIZE];
            _SendNextChunk();

            var recvBuffer = new byte[_BUFSIZE];
            int bytesRcvd;
            while ((bytesRcvd = _ClientSock.Receive(recvBuffer)) > 0)
            {
                fileOut.Write(recvBuffer, 0, bytesRcvd);
                Console.Write("R");
            }

            Console.WriteLine("\nTransformation Complete.");

            fileOut.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            _FileIn.Close();
        }
    }

    private static void _SendNextChunk()
    {
        try
        {
            int bytesRead = _FileIn.Read(_SendBuffer, 0, _SendBuffer.Length);

            if (bytesRead > 0)
            {
                _ClientSock.BeginSend(_SendBuffer, 0, bytesRead, SocketFlags.None, _SendCallback, null);
            }
            else
            {
                _ClientSock.Shutdown(SocketShutdown.Send);
                _FileIn.Close();
                Console.WriteLine("\n[Send Complete, Socket Half-Closed]");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("\nError in sending: " + e.Message);
            _FileIn.Close();
            _ClientSock.Close();
        }
    }

    private static void _SendCallback(IAsyncResult asyncResult)
    {
        try
        {
            int bytesSent = _ClientSock.EndSend(asyncResult);
            Console.Write("W");

            _SendNextChunk();
        }
        catch (Exception e)
        {
            Console.WriteLine("\nError in SendCallback: " + e.Message);
            _FileIn.Close();
            _ClientSock.Close();
        }
    }
}