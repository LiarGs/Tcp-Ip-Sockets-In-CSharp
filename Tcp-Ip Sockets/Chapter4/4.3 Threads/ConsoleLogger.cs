using System.Collections;

namespace TcpIpSocketsLearn.Chapter4;

internal class ConsoleLogger : ILogger
{
    private static Mutex _Mutex = new Mutex();

    public void WriteEntry(ArrayList entry)
    {
        _Mutex.WaitOne();

        var line = entry.GetEnumerator();
        while (line.MoveNext())
        {
            Console.WriteLine(line.Current);
        }

        Console.WriteLine();

        _Mutex.ReleaseMutex();
    }

    public void WriteEntry(string entry)
    {
        _Mutex.WaitOne();

        Console.WriteLine(entry);
        Console.WriteLine();

        _Mutex.ReleaseMutex();
    }
}