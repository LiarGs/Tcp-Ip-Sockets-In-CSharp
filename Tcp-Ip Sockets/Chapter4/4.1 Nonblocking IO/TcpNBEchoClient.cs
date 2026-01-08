using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpIpSocketsLearn.Chapter4;

public static class TcpNBEchoClient
{
    public static void Example(string[] args)
    {
        if ((args.Length < 2) || (args.Length > 3)) // Test for correct # of args
            throw new ArgumentException("Parameters: <Server> <Word> [<Port>]");

        string server = args[0]; // Server name or IP address

        // Convert input String to bytes
        byte[] byteBuffer = Encoding.ASCII.GetBytes(args[1]);

        // Use port argument if supplied, otherwise default to 7
        int servPort = (args.Length == 3) ? int.Parse(args[2]) : 7;

        // Create Socket and connect
        Socket? sock = null;
        try
        {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipV4Address = Dns.GetHostEntry(server).AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            sock.Connect(new IPEndPoint(ipV4Address, servPort));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Environment.Exit(-1);
        }

        // Receive the same string back from the server
        int totalBytesSent = 0; // Total bytes sent so far
        int totalBytesRcvd = 0; // Total bytes received so far

        // Make sock a nonblocking Socket
        sock.Blocking = false;

        // Loop until all bytes have been echoed by server
        while (totalBytesRcvd < byteBuffer.Length)
        {
            // Send the encoded string to the server
            if (totalBytesSent < byteBuffer.Length)
            {
                try
                {
                    totalBytesSent += sock.Send(byteBuffer, totalBytesSent,
                        byteBuffer.Length - totalBytesSent, SocketFlags.None);
                    Console.WriteLine("Sent a total of {0} bytes to server...", totalBytesSent);
                }
                catch (SocketException se)
                {
                    if (se.ErrorCode == 10035)
                    {
                        //WSAEWOULDBLOCK: Resource temporarily unavailable
                        Console.WriteLine("Temporarily unable to send, will retry again later.");
                    }
                    else
                    {
                        Console.WriteLine(se.ErrorCode + ": " + se.Message);
                        sock.Close();
                        Environment.Exit(se.ErrorCode);
                    }
                }
            }

            try
            {
                int bytesRcvd = 0;
                if ((bytesRcvd = sock.Receive(byteBuffer, totalBytesRcvd,
                        byteBuffer.Length - totalBytesRcvd, SocketFlags.None)) == 0)
                {
                    Console.WriteLine("Connection closed prematurely.");
                    break;
                }

                totalBytesRcvd += bytesRcvd;
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10035) // WSAEWOULDBLOCK: Resource temporarily unavailable
                    continue;

                Console.WriteLine(se.ErrorCode + ": " + se.Message);
                break;
            }

            _DoThing();
        }

        Console.WriteLine("Received {0} bytes from server: {1}", totalBytesRcvd,
            Encoding.ASCII.GetString(byteBuffer, 0, totalBytesRcvd));

        sock.Close();
    }

    private static void _DoThing()
    {
        Console.Write(".");
        Thread.Sleep(2000);
    }
}