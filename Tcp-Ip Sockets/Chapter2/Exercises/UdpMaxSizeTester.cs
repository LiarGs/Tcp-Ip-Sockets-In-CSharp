using System.Net;
using System.Net.Sockets;

namespace TcpIpSockets.Chapter2;

internal static class UdpMaxSizeTester
{
    public static void Example(string server = "127.0.0.1", int servPort = 7)
    {
        Console.WriteLine($"Testing UDP Max Message Size against {server}:{servPort}...");

        using var client = new UdpClient();
        client.Client.ReceiveTimeout = 2000;

        var remoteEndPoint = new IPEndPoint(IPAddress.Parse(server), servPort);

        int[] testSizes = { 65500, 65506, 65507, 65508, 65535 };

        foreach (var size in testSizes)
        {
            Console.Write($"\nTesting payload size: {size} bytes... ");

            try
            {
                var data = new byte[size];
                data[0]        = 0x01;
                data[size - 1] = 0x02;

                client.Send(data, data.Length, remoteEndPoint);
                Console.Write("Sent! ");

                var serverEp     = new IPEndPoint(IPAddress.Any, 0);
                var receivedData = client.Receive(ref serverEp);

                if (receivedData.Length == size)
                    Console.Write("Received Echo Success!");
                else
                    Console.Write($"Received partial/wrong data: {receivedData.Length}");
            }
            catch (SocketException se)
            {
                Console.Write($"Failed! Error: {se.ErrorCode} ({se.SocketErrorCode})");
                if (se.SocketErrorCode == SocketError.MessageSize) Console.Write(" -> Message too long (Expected)");
            }
            catch (Exception e)
            {
                Console.Write($"Error: {e.Message}");
            }
        }

        Console.WriteLine("\n\nTest Finished.");
    }
}