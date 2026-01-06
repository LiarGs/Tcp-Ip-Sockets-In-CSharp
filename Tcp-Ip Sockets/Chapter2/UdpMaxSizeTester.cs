using System.Net;
using System.Net.Sockets;

namespace TcpIpSockets.Chapter2;

internal static class UdpMaxSizeTester
{
    public static void Example(string server = "127.0.0.1", int servPort = 7)
    {
        Console.WriteLine($"Testing UDP Max Message Size against {server}:{servPort}...");

        using var client = new UdpClient();
        client.Client.ReceiveTimeout = 2000; // 设置2秒超时

        var remoteEndPoint = new IPEndPoint(IPAddress.Parse(server), servPort);

        // 我们从理论极限附近开始测试
        // 65507 是 IPv4 的理论最大值
        int[] testSizes = { 65500, 65506, 65507, 65508, 65535 };

        foreach (var size in testSizes)
        {
            Console.Write($"\nTesting payload size: {size} bytes... ");

            try
            {
                var data = new byte[size];
                // 填充一些假数据，以免为空
                data[0]        = 0x01;
                data[size - 1] = 0x02;

                // 1. 尝试发送
                client.Send(data, data.Length, remoteEndPoint);
                Console.Write("Sent! ");

                // 2. 尝试接收回显 (验证服务器是否收到并且我们能否收回)
                var serverEp     = new IPEndPoint(IPAddress.Any, 0);
                var receivedData = client.Receive(ref serverEp);

                if (receivedData.Length == size)
                    Console.Write("Received Echo Success!");
                else
                    Console.Write($"Received partial/wrong data: {receivedData.Length}");
            }
            catch (SocketException se)
            {
                // 捕获具体的错误代码
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