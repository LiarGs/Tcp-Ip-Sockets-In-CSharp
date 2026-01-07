using System.Net.Sockets;

namespace TcpIpSocketsLearn.Chapter4;

public static class TranscodeClient
{
    private const int BUFSIZE = 256;

    private static NetworkStream     _NetStream;
    private static FileStream        _FileIn;
    private static TcpClientShutdown _Client;

    public static void Example(string[] args)
    {
        if (args.Length != 3)
            throw new ArgumentException("Parameter(s): <Server> <Port> <File>");

        var server   = args[0];
        var port     = int.Parse(args[1]);
        var filename = args[2];

        _FileIn = new FileStream(filename, FileMode.Open, FileAccess.Read);
        var fileOut = new FileStream(filename + ".ut8", FileMode.Create);

        _Client = new TcpClientShutdown();
        _Client.Connect(server, port);

        _NetStream = _Client.GetStream();
        _SendBytes();

        int bytesRead;
        var buffer = new byte[BUFSIZE];
        while ((bytesRead = _NetStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            fileOut.Write(buffer, 0, bytesRead);
            Console.Write("R");
        }

        Console.WriteLine();

        _NetStream.Close();
        _Client.Close();
        _FileIn.Close();
        fileOut.Close();
    }

    private static void _SendBytes()
    {
        int bytesRead;
        var fileInBuf = new BufferedStream(_FileIn);
        var buffer    = new byte[BUFSIZE];
        while ((bytesRead = fileInBuf.Read(buffer, 0, buffer.Length)) > 0)
        {
            _NetStream.Write(buffer, 0, bytesRead);
            Console.Write("W");
        }

        _Client.Shutdown(SocketShutdown.Send);
    }
}