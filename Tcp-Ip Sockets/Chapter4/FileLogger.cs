using System.Collections;

namespace TcpIpSocketsLearn.Chapter4;

internal class FileLogger : ILogger
{
    private static Mutex _Mutex = new Mutex();

    private StreamWriter _Output; // Log file

    public FileLogger(string filename)
    {
        // Create log file
        _Output = new StreamWriter(filename, true);
    }

    public void WriteEntry(ArrayList entry)
    {
        _Mutex.WaitOne();

        var line = entry.GetEnumerator();
        while (line.MoveNext())
            _Output.WriteLine(line.Current);

        _Output.WriteLine();
        _Output.Flush();

        _Mutex.ReleaseMutex();
    }

    public void WriteEntry(string entry)
    {
        _Mutex.WaitOne();

        _Output.WriteLine(entry);
        _Output.WriteLine();
        _Output.Flush();

        _Mutex.ReleaseMutex();
    }
}