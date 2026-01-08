using System.Collections;

namespace TcpIpSocketsLearn.Chapter4;

public interface ILogger
{
    void WriteEntry(ArrayList entry); // Write list of lines
    void WriteEntry(string    entry); // Write single line
}